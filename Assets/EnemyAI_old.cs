using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Attack }
    public EnemyState currentState;

    private NavMeshAgent agent;
    private Transform playerTarget;

    public Transform[] patrolPoints;
    private int destPoint = 0;

    public float sightRange = 20f;
    public float attackRange = 5f;
    public float fieldOfViewAngle = 90f;
    public float eyeLevelHeight = 1.5f;

    public float timeBetweenAttacks = 1f;
    public int attackDamage = 10;
    private bool alreadyAttacked;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoRepath = true;
        agent.autoBraking = false;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTarget = playerObj.transform;

        if (patrolPoints.Length > 0)
        {
            destPoint = 0;
            agent.SetDestination(patrolPoints[destPoint].position);
        }

        currentState = EnemyState.Patrol;
    }

    void Update()
    {
        if (playerTarget == null) return;

        CheckTransitions();

        switch (currentState)
        {
            case EnemyState.Idle:
                IdleBehavior();
                break;
            case EnemyState.Patrol:
                PatrolBehavior();
                break;
            case EnemyState.Chase:
                ChaseBehavior();
                break;
            case EnemyState.Attack:
                AttackBehavior();
                break;
        }
    }

    bool IsPlayerVisible()
    {
        float distanceToPlayer = Vector3.Distance(playerTarget.position, transform.position);
        if (distanceToPlayer > sightRange) return false;

        Vector3 eyeLevelStart = transform.position + Vector3.up * eyeLevelHeight;
        Vector3 targetPosition = playerTarget.position + Vector3.up * 1f;
        Vector3 directionToTarget = (targetPosition - eyeLevelStart).normalized;

        if (Vector3.Angle(transform.forward, directionToTarget) < fieldOfViewAngle / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(eyeLevelStart, directionToTarget, out hit, sightRange))
            {
                if (hit.transform == playerTarget)
                    return true;
            }
        }
        return false;
    }


    void IdleBehavior()
    {
        agent.SetDestination(transform.position);
    }

    void PatrolBehavior()
    {
        if (patrolPoints.Length == 0)
        {
            currentState = EnemyState.Idle;
            return;
        }

        agent.speed = 3.5f;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            destPoint = (destPoint + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[destPoint].position);
        }
    }

    void ChaseBehavior()
    {
        agent.speed = 5f;

        if (playerTarget != null)
        {
            agent.SetDestination(playerTarget.position);

            Vector3 lookAtTarget = playerTarget.position;
            lookAtTarget.y = transform.position.y;
            transform.LookAt(lookAtTarget);
        }
    }

    void AttackBehavior()
    {
        agent.SetDestination(transform.position);

        Vector3 lookAtTarget = playerTarget.position;
        lookAtTarget.y = transform.position.y;
        transform.LookAt(lookAtTarget);

        if (!alreadyAttacked)
        {
            PlayerHealth playerHealth = playerTarget.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log("Bot saldýrdý! " + attackDamage + " hasar verdi.");
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    void CheckTransitions()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        bool canSeePlayer = IsPlayerVisible();

        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
            return;
        }

        if (canSeePlayer)
        {
            currentState = EnemyState.Chase;
            return;
        }

        if (currentState == EnemyState.Chase && !canSeePlayer)
        {
            agent.ResetPath();
            if (patrolPoints.Length > 0)
                agent.SetDestination(patrolPoints[destPoint].position);

            currentState = EnemyState.Patrol;
            return;
        }

        if (currentState != EnemyState.Patrol)
            currentState = EnemyState.Patrol;
    }
}
