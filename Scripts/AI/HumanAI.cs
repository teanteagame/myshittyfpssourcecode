using System.Collections;
using UnityEngine;

public class HumanAI : AIBase
{
    [Header("Human AI Setting")]
    public float safeRange = 10; //10 meter away from the enemy

    [Header("Aim IK")]
    public Transform aimTransform;
    public HumanBone[] humanBones;
    public int iterations = 10;
    [Range(0, 1)] public float weight = 1;
    public float angleLimit = 90;
    public float distanceLimit = 1;

    public GameObject weaponObject;
    public Transform ikTransform;

    private bool isAiming;
    private Transform[] boneTransforms;

    public override void AIUpdate()
    {
        if (isAiming)
        {
            if (weaponObject)
            {
                if (!weaponObject.activeInHierarchy) weaponObject.SetActive(true);
            }

            Vector3 targetPosition = GetTargetPosition();
            for (int i = 0; i < iterations; i++)
            {
                for (int b = 0; b < boneTransforms.Length; b++)
                {
                    AimIK(boneTransforms[b], targetPosition, weight * humanBones[b].weight);
                }
            }
        }
        else
        {
            if (weaponObject)
            {
                if (weaponObject.activeInHierarchy) weaponObject.SetActive(false);
            }
        }

        //animator.SetFloat("Speed", agent.velocity.magnitude / walkSpeed);
        ///animator.SetBool("Aim", isAiming);
    }

    public override void Idle()
    {
        //Random patrol or random special animation

        agent.speed = Mathf.Lerp(agent.speed, 0, Time.deltaTime * 10);
        agent.isStopped = true;
        isAiming = false;

        //if see the enemy
        if (currentEnemy == null && ClosestEnemy())
        {
            currentEnemy = ClosestEnemy();
            currentState = AiState.Chase;
        }
    }

    public override void Patrol()
    {
        agent.speed = Mathf.Lerp(agent.speed, walkSpeed, Time.deltaTime * 10);
        agent.isStopped = false;
        isAiming = false;

        //Random rest time

        //Random wandering
        //Random wandering       
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= wanderInterval)
        {
            wanderPosition = transform.position + Random.insideUnitSphere * wanderRadius;
            wanderTimer = 0;
        }

        MoveToPosition(wanderPosition);
        LookAtPosition(transform.position + agent.desiredVelocity);

        //if see the enemy
        if (currentEnemy == null && ClosestEnemy())
        {
            currentEnemy = ClosestEnemy();
            currentState = AiState.Chase;
        }
    }

    public override void Chase()
    {
        agent.speed = Mathf.Lerp(agent.speed, runSpeed, Time.deltaTime * 5);
        agent.isStopped = false;
        isAiming = true;
       
        if (currentEnemy)
        {
            //Check the distance if within the attack range                         
            if (EnemyInAttackRange())
            {
                if (IsInSafeRange())
                {
                    currentState = AiState.Attack;
                }
                else
                {
                    Vector3 normDir = (currentEnemy.transform.position - transform.position).normalized;

                    normDir = Quaternion.AngleAxis(Random.Range(0, 179), Vector3.up) * normDir;

                    MoveToPosition(transform.position - (normDir * safeRange));
                    LookAtPosition(currentEnemy.transform.position);
                }
            }
            else
            {
                MoveToPosition(currentEnemy.transform.position);
                LookAtPosition(transform.position + agent.desiredVelocity);
            }
           
    
            //if see the enemy  
            if (!IsWatchingEnemy())
            {
                loseTimer += Time.deltaTime;
                if (loseTimer >= loseThreshold)
                {
                    currentEnemy = null;
                    loseTimer = 0;
                }
                //MoveToPosition(currentEnemy.transform.position);
            }
            else
            {
                loseTimer = 0;
            }                       
        }
        else
        {
            currentState = AiState.Patrol;            
        }
    }

    public override void Attack()
    {
        //Validate the target(position)
        agent.speed = 0;
        agent.isStopped = true;

        isAiming = true;

        if (!isAttacking)
        {
            isAttacking = true;
            StartCoroutine(AttackCoroutine());
        }
    }

    public override void Hit()
    {
        agent.speed = 0;
        agent.isStopped = true;
    }  

    public override void TakeDamage(float amount, Vector3 fromPos)
    {
        
    }

    IEnumerator AttackCoroutine()
    {
        //Damage the target

        animator.CrossFadeInFixedTime("Fire", 0.1f);


        yield return new WaitForSeconds(attackDelay);
        isAttacking = false;
        currentState = AiState.Chase;
    }

    public override IEnumerator WakeUp()
    {
        yield return base.WakeUp();    
        boneTransforms = new Transform[humanBones.Length];
        for (int i = 0; i < boneTransforms.Length; i++)
        {
            boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
        }
        yield return null;
    }

    public override IEnumerator TraverseOffmesh()
    {
        isAiming = false;
        yield return base.TraverseOffmesh();        
    }

    private void AimIK(Transform bone, Vector3 targetPosition, float weight)
    {
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection = targetPosition - aimTransform.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        bone.rotation = blendedRotation * bone.rotation;
    }

    Vector3 GetTargetPosition()
    {
        Vector3 tarPos = currentEnemy ? (currentEnemy.transform.position + currentEnemy.detectOffset) : eyePos.forward * 10;
        Vector3 targetDirection = tarPos - aimTransform.position;
        Vector3 aimDirection = aimTransform.forward;
        float blendOut = 0.0f;
        float targetAngle = Vector3.Angle(targetDirection, aimDirection);
        if (targetAngle > angleLimit)
        {
            blendOut += (targetAngle - angleLimit) / 50.0f;
        }
        float targetDistance = targetDirection.magnitude;
        if (targetDistance < distanceLimit)
        {
            blendOut += distanceLimit - targetDistance;

        }
        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
        return aimTransform.position + direction;
    }

    private void OnAnimatorIK()
    {
        if (animator == null) return;
        if (isAiming)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, ikTransform.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, ikTransform.rotation);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(currentEnemy ? currentEnemy.transform.position : eyePos.position, eyePos.position);

        Gizmos.DrawLine(eyePos.position, eyePos.position + eyePos.forward * seeDistance);
        if (weaponObject) Gizmos.DrawRay(weaponObject.transform.position, weaponObject.transform.forward * attackRange);
    }

    private bool IsInSafeRange()
    {
        if (Vector3.Distance(currentEnemy.transform.position, transform.position) > safeRange)
        {
            return true;
        }
        return false;
    }
}
