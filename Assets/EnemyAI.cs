using UnityEngine;
using UnityEngine.AI;

namespace EnemySystem
{
    public class EnemyAI : MonoBehaviour
    {
        [HideInInspector] public NavMeshAgent agent;
        [HideInInspector] public Transform playerTarget;

        public EnemyState currentState;

        [Header("Devriye Ayarlarý")]
        public Transform[] patrolPoints;
        [HideInInspector] public int currentPatrolIndex = 0;
        public float patrolSpeed = 3.5f;

        [Header("Görüþ ve Mesafe Ayarlarý")]
        public float sightRange = 20f;
        public float attackRange = 5f;
        public float fieldOfViewAngle = 90f;
        public float eyeLevelHeight = 1.5f;

        [Header("Saldýrý Ayarlarý")]
        public float chaseSpeed = 5f;
        public float timeBetweenAttacks = 1f;
        public int attackDamage = 10;
        [HideInInspector] public bool alreadyAttacked;

        public enum EnemyState { Idle, Patrol, Chase, Attack }

        void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.autoRepath = true;
                agent.autoBraking = false;
            }

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTarget = playerObj.transform;
        }

        void Start()
        {
            currentState = EnemyState.Patrol;
            if (patrolPoints.Length > 0 && agent != null)
            {
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
            }
        }

        void Update()
        {
            if (playerTarget == null) return;

            CheckTransitions();
            ExecuteCurrentState();
        }

        private void CheckTransitions()
        {
            EnemyState nextState = EnemyTransitionChecker.GetNextState(this);
            if (nextState != currentState)
            {
                currentState = nextState;
            }
        }

        private void ExecuteCurrentState()
        {
            switch (currentState)
            {
                case EnemyState.Idle:
                    EnemyBehaviors.Idle(this);
                    break;
                case EnemyState.Patrol:
                    EnemyBehaviors.Patrol(this);
                    break;
                case EnemyState.Chase:
                    EnemyBehaviors.Chase(this);
                    break;
                case EnemyState.Attack:
                    EnemyBehaviors.Attack(this);
                    break;
            }
        }

        public void ResetAttack()
        {
            alreadyAttacked = false;
        }
    }
}