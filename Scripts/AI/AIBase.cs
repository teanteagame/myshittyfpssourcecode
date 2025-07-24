using UnityEngine.AI;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(NavMeshAgent))]
public class AIBase : FactionObject
{
    [Header("Brain")]
    public Transform eyePos;
    public FactionBehave[] factionBehaves;
    public LayerMask searchLayers;
    public LayerMask attackLayers;

    [Header("Stats")]
    public float health = 100;
    public float maxHealth = 100;

    [Header("States")]
    public AiState currentState;

    [Header("Movement")]
    public float walkSpeed = 2;
    public float runSpeed = 4;
    public float maxTime = 0.5f;
    public float maxDistance = 1;
    public float rotationSpeed = 10;

    [Header("Animation")]
    public float readyTime = 3;

    [Header("Attack")]
    public float attackRange = 2;
    public float attackDelay = 1; // Before damaging target
    public float attackCoolDown = 3; // Wait untill your next attack

    [Header("Patrol")]
    public float maxIdleTime = 5; //Idle for 5s the go back to patrol;
    public float maxPatrolTime = 10; //Patrol for 10s then rest(idle)
    public float wanderInterval = 5.0f; // Time between choosing new wander targets (seconds)
    public float wanderRadius = 10.0f; // Radius of the wandering area    

    //Should be moved
    [Header("Detection")]
    public float fov = 120;
    public float seeDistance = 10;
    public float hearDistance = 30;
    public float loseThreshold = 10;
    public float scanFrequency = 0.5f;//scan every 0.5s

    [Header("Off Mesh Action")]
    public List<OffMeshActionType> offMeshActionTypes = new();
    protected bool hasTraversed;
    protected string currentAnimatorBool;
    protected float currentTransitDuration;
    protected AnimationCurve currentOffmeshCurve;
    protected OffMeshLinkData offmeshData;
    protected Vector3 offmeshStartPos;
    protected Vector3 offmeshEndPos;

    protected float moveTimer;
    protected float loseTimer;
    protected float scanTimer;
    protected float idleTimer;
    protected float patrolTimer;
    protected float wanderTimer;

    #region Network Variables
    protected Vector3 syncPos;
    protected Quaternion syncRot;
    protected float magnitude;
    #endregion

    protected List<FactionObject> visibleObjects = new();
    protected List<FactionObject> enemyObjects = new();
    protected FactionObject currentEnemy;

    protected NavMeshAgent agent;
    protected Animator animator;
    protected FactionManager factionManager;

    protected bool isReady;
    protected bool isAttacking;

    protected Vector3 wanderPosition;

    public override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        factionManager = FactionManager.instance;
        StartCoroutine(WakeUp());
    }

    private void Update()
    {
        if (!isReady) return;
        if (agent.isOnOffMeshLink)
        {
            if (!hasTraversed)
            {
                StartCoroutine(TraverseOffmesh());
            }
        }
        else
        {
            AIUpdate();
            switch (currentState)
            {
                case AiState.Idle:
                    Idle();
                    break;
                case AiState.Patrol:
                    Patrol();
                    break;
                case AiState.Chase:
                    Chase();
                    break;
                case AiState.Attack:
                    Attack();
                    break;
                case AiState.Hit:
                    Hit();
                    break;
            }
            ScanFactionObjects();
        }
    }

    private void FixedUpdate()
    {
       
    }

    public virtual void Idle() { }
    public virtual void Patrol() { }
    public virtual void Chase() { }
    public virtual void Attack() { }
    public virtual void Hit() { }
    public virtual void AIUpdate() { }
    public override void TakeDamage(float amount, Vector3 fromPos) { }

    public void MoveToPosition(Vector3 pos)
    {
        moveTimer -= Time.deltaTime;
        if (moveTimer < 0)
        {
            float sqrDis = (pos - agent.transform.position).sqrMagnitude;
            if (sqrDis > maxDistance)
            {
                agent.destination = pos;
            }
            moveTimer = maxTime;
        }
    }

    public void LookAtPosition(Vector3 pos)
    {
        pos.y = transform.position.y;
        Vector3 targetDirection = pos - transform.position;
        if (targetDirection == Vector3.zero) return;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        targetRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        transform.rotation = targetRotation;
    }

    private void ScanFactionObjects()
    {
        scanTimer += Time.deltaTime;
        if (scanTimer >= scanFrequency)
        {
            DecideNewTargets();
            scanTimer = 0;
        }
    }

    public void DecideNewTargets()
    {
        visibleObjects.Clear();
        if (factionManager == null) return;
        foreach (FactionObject faction in factionManager.objectFactions)
        {
            if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(faction.transform.position)) < fov / 2)
            {
                if (Vector3.Distance(faction.transform.position, transform.position) < seeDistance)
                {
                    if (Physics.Linecast(eyePos.position, faction.transform.position + faction.detectOffset, out RaycastHit hit, searchLayers))
                    {
                        if (hit.transform.GetComponent<FactionObject>())
                        {
                            if (hit.transform.GetComponent<FactionObject>() != GetComponent<FactionObject>())
                                visibleObjects.Add(hit.transform.GetComponent<FactionObject>());
                        }
                    }
                }
            }
        }

        GetEnemies();
    }

    public void GetEnemies()
    {
        enemyObjects.Clear();
        if (visibleObjects.Count <= 0) return;
        foreach (FactionObject faction in visibleObjects)
        {
            if (IsEnemy(faction.factionNumber))
            {
                enemyObjects.Add(faction);
            }
        }
    }

    private void PrepareForOffMeshLink()
    {
        offmeshData = agent.currentOffMeshLinkData;
        //offmeshStartPos = agent.transform.position;
        offmeshStartPos = offmeshData.startPos;
        //+ agent.baseOffset * Vector3.up
        offmeshEndPos = offmeshData.endPos + agent.baseOffset * Vector3.up;
        hasTraversed = true;
        IdentifyOffmeshLink();
        Debug.Log("Prepare AI For Offmesh");
    }

    private void IdentifyOffmeshLink()
    {
        Transform startTransform = offmeshData.offMeshLink.startTransform;
        Transform endTransform = offmeshData.offMeshLink.endTransform;
        float startDis = Vector3.Distance(agent.transform.position, startTransform.position);
        float endDis = Vector3.Distance(agent.transform.position, endTransform.position);
        if (startDis < endDis)
        {
            currentAnimatorBool = offmeshData.offMeshLink.transform.GetComponent<OffMeshAction>().startBoolName;
        }
        else if (endDis < startDis)
        {
            currentAnimatorBool = offmeshData.offMeshLink.transform.GetComponent<OffMeshAction>().endBoolName;
        }
        for (int i = 0; i < offMeshActionTypes.Count; i++)
        {
            if (offMeshActionTypes[i].AnimatorBool == currentAnimatorBool)
            {
                currentOffmeshCurve = offMeshActionTypes[i].offmeshCurve;
                currentTransitDuration = offMeshActionTypes[i].TransitDuration;
                break;
            }
        }
    }

    public bool IsWatchingEnemy()
    {
        if (currentEnemy)
        {
            if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(currentEnemy.transform.position)) < fov / 2)
            {
                if (Vector3.Distance(currentEnemy.transform.position, transform.position) < seeDistance)
                {
                    if (Physics.Linecast(eyePos.position, currentEnemy.transform.position + currentEnemy.detectOffset, out RaycastHit hit, searchLayers))
                    {
                        if (hit.transform.GetComponent<FactionObject>())
                        {
                            if (hit.transform.GetComponent<FactionObject>() == currentEnemy)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool EnemyInAttackRange()
    {
        if (currentEnemy)
        {
            if (Vector3.Angle(Vector3.forward, transform.InverseTransformPoint(currentEnemy.transform.position)) < fov / 2)
            {
                if (Vector3.Distance(currentEnemy.transform.position, transform.position) < attackRange)
                {
                    if (Physics.Linecast(eyePos.position, currentEnemy.transform.position + currentEnemy.detectOffset, out RaycastHit hit, attackLayers))
                    {
                        if (hit.transform.GetComponent<FactionObject>())
                        {
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool IsEnemy(int factionNumber)
    {
        if (GetFaction(factionNumber) != null)
        {
            if (GetFaction(factionNumber).factionRole == FactionRole.Enemy)
            {
                return true;
            }
        }

        return false;
    }

    public FactionBehave GetFaction(int factionNumber)
    {
        for (int i = 0; i < factionBehaves.Length; i++)
        {
            if (factionBehaves[i].factionNumber == factionNumber)
            {
                return factionBehaves[i];
            }
        }
        return null;
    }

    public FactionObject ClosestEnemy()
    {
        if (enemyObjects.Count <= 0)
        {
            return null;
        }
        else
        {
            float closestDis = float.PositiveInfinity;
            foreach (FactionObject faction in enemyObjects)
            {
                float dis = Vector3.Distance(faction.transform.position, transform.position);
                if (dis < closestDis)
                {
                    closestDis = dis;
                    return faction;
                }
            }
            return null;
        }
    }

    protected Vector3 GetClosestPointOnNavMesh(Vector3 point)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(point, out hit, 1f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            // Handle case where point is not on NavMesh (e.g., log error, find alternative)
            return point;
        }
    }

    public virtual IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(readyTime);
        agent.updateRotation = false;
        agent.autoTraverseOffMeshLink = false;
        isReady = true;
    }

    public virtual IEnumerator TraverseOffmesh()
    {
        PrepareForOffMeshLink();
        animator.SetBool(currentAnimatorBool, hasTraversed);
        float time = 0f;
        while (time <= currentTransitDuration)
        {
            LookAtPosition(offmeshEndPos);
            if (agent == null)
            {
                yield return null;
            }
            Vector3 curveValue = currentOffmeshCurve.Evaluate(time / currentTransitDuration) * Vector3.up;
            agent.transform.position = Vector3.Lerp(offmeshStartPos, offmeshEndPos, time / currentTransitDuration) + curveValue;
            time += Time.deltaTime;
            yield return null;
        }
        agent.CompleteOffMeshLink();
        hasTraversed = false;
        animator.SetBool(currentAnimatorBool, hasTraversed);
    }
}

[System.Serializable]
public class FactionBehave
{
    [Range(0, 99)] public int factionNumber = 1;
    public FactionRole factionRole;
}

[Serializable]
public class OffMeshActionType
{     
    public string AnimatorBool = "Vault";

    public AnimationCurve offmeshCurve = new(new Keyframe[0]);

    public float TransitDuration = 1.5f;
}

public enum AiState
{
    Idle, Patrol, Chase, Attack, Hit
}

public enum FactionRole
{
    Companion, //Follow and assist this faction
    Neutral, // Won't attack this faction, unless it attack first
    Enemy, //Attack this faction on range
    Threath//Runaway from this faction
}