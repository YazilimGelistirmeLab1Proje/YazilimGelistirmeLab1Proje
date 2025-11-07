using UnityEngine;

namespace EnemySystem
{
    public static class EnemyTransitionChecker
    {
        public static EnemyAI.EnemyState GetNextState(EnemyAI ai)
        {
            if (ai.playerTarget == null) return ai.currentState;

            float distanceToPlayer = Vector3.Distance(ai.transform.position, ai.playerTarget.position);
            bool canSeePlayer = IsPlayerVisible(ai);

            if (distanceToPlayer <= ai.attackRange)
            {
                return EnemyAI.EnemyState.Attack;
            }

            if (canSeePlayer)
            {
                return EnemyAI.EnemyState.Chase;
            }

            if (ai.currentState == EnemyAI.EnemyState.Chase && !canSeePlayer)
            {
                if (ai.agent != null)
                {
                    ai.agent.ResetPath();
                    if (ai.patrolPoints.Length > 0)
                        ai.agent.SetDestination(ai.patrolPoints[ai.currentPatrolIndex].position);
                }
                return EnemyAI.EnemyState.Patrol;
            }

            if (ai.currentState != EnemyAI.EnemyState.Patrol && ai.patrolPoints.Length > 0)
            {
                return EnemyAI.EnemyState.Patrol;
            }

            return ai.currentState;
        }

        private static bool IsPlayerVisible(EnemyAI ai)
        {
            if (ai.playerTarget == null) return false;

            float distanceToPlayer = Vector3.Distance(ai.playerTarget.position, ai.transform.position);
            if (distanceToPlayer > ai.sightRange) return false;

            Vector3 eyeLevelStart = ai.transform.position + Vector3.up * ai.eyeLevelHeight;
            Vector3 targetPosition = ai.playerTarget.position + Vector3.up * 1f;
            Vector3 directionToTarget = (targetPosition - eyeLevelStart).normalized;

            if (Vector3.Angle(ai.transform.forward, directionToTarget) < ai.fieldOfViewAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(eyeLevelStart, directionToTarget, out hit, ai.sightRange))
                {
                    if (hit.transform == ai.playerTarget)
                        return true;
                }
            }
            return false;
        }
    }
}