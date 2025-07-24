using System.Collections;
using UnityEngine;

public class ZombieAI : AIBase
{      
    public override void AIUpdate()
    {           
        //animator.SetFloat("Speed", agent.velocity.magnitude / walkSpeed);       
    }

    public override void Idle()
    {
        //Random patrol or random special animation

        agent.speed = Mathf.Lerp(agent.speed, 0, Time.deltaTime * 10);
        agent.isStopped = true;

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

        //Random rest time

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

        if (currentEnemy)
        {
            //Check the distance if within the attack range                         
            if (EnemyInAttackRange())
            {
                currentState = AiState.Attack; 
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
                if(loseTimer >= loseThreshold)
                {
                    currentEnemy = null;
                    loseTimer = 0;
                }
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
        animator.CrossFadeInFixedTime("Attack 5", 0.1f);
        yield return new WaitForSeconds(attackDelay);
        CheckSphereCastHit();
        yield return new WaitForSeconds(attackCoolDown);
        isAttacking = false;
        currentState = AiState.Chase;
    }

    void CheckSphereCastHit()
    {
        RaycastHit[] hits = Physics.SphereCastAll(eyePos.position, 0.3f, eyePos.forward, attackRange, attackLayers);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.GetComponent<DamageableObject>() && hits[i].point != Vector3.zero)
            {
                DamageableObject damageable = hits[i].transform.gameObject.GetComponent<DamageableObject>();
                damageable.TakeDamage(10, transform.position);
            }

            if (hits[i].transform.gameObject.GetComponent<Rigidbody>() && hits[i].point != Vector3.zero)
            {
                Rigidbody rigid = hits[i].transform.gameObject.GetComponent<Rigidbody>();
                if (!rigid.isKinematic) rigid.AddForce(-hits[i].normal * 30);
            }

            //if (hits[i].point != Vector3.zero) CreateParticleAtPoint(hits[i].point, Quaternion.LookRotation(hits[i].normal), hits[i].transform.tag);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(currentEnemy ? currentEnemy.transform.position : eyePos.position, eyePos.position);

        Gizmos.DrawLine(eyePos.position, eyePos.position + eyePos.forward * seeDistance);
    }
}
