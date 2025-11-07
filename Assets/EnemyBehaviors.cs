using UnityEngine;

namespace EnemySystem
{
    public static class EnemyBehaviors
    {
        public static void Idle(EnemyAI ai)
        {
            if (ai.agent != null)
                ai.agent.SetDestination(ai.transform.position);
        }

        public static void Patrol(EnemyAI ai)
        {
            if (ai.patrolPoints.Length == 0)
            {
                ai.currentState = EnemyAI.EnemyState.Idle;
                return;
            }

            if (ai.agent == null) return;

            ai.agent.speed = ai.patrolSpeed;

            if (!ai.agent.pathPending && ai.agent.remainingDistance <= ai.agent.stoppingDistance + 0.2f)
            {
                ai.currentPatrolIndex = (ai.currentPatrolIndex + 1) % ai.patrolPoints.Length;
                ai.agent.SetDestination(ai.patrolPoints[ai.currentPatrolIndex].position);
            }
        }

        public static void Chase(EnemyAI ai)
        {
            if (ai.agent == null || ai.playerTarget == null) return;

            ai.agent.speed = ai.chaseSpeed;

            ai.agent.SetDestination(ai.playerTarget.position);

            Vector3 lookAtTarget = ai.playerTarget.position;
            lookAtTarget.y = ai.transform.position.y;
            ai.transform.LookAt(lookAtTarget);
        }

        public static void Attack(EnemyAI ai)
        {
            if (ai.agent != null)
                ai.agent.SetDestination(ai.transform.position);

            if (ai.playerTarget != null)
            {
                Vector3 lookAtTarget = ai.playerTarget.position;
                lookAtTarget.y = ai.transform.position.y;
                ai.transform.LookAt(lookAtTarget);
            }

            if (!ai.alreadyAttacked)
            {
                if (ai.playerTarget != null)
                {
                    PlayerHealth playerHealth = ai.playerTarget.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(ai.attackDamage);
                        Debug.Log("Bot saldýrdý! " + ai.attackDamage + " hasar verdi.");
                    }
                }

                ai.alreadyAttacked = true;
                ai.Invoke(nameof(ai.ResetAttack), ai.timeBetweenAttacks);
            }
        }
    }
}