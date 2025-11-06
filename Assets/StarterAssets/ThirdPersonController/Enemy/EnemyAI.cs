using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // --- A. Durumlar ---
    public enum EnemyState { Idle, Patrol, Chase, Attack }
    public EnemyState currentState;

    // --- B. Bileþenler ---
    private NavMeshAgent agent;
    private Transform playerTarget;
    private Animator animator; // Animasyon için

    // --- C. Devriye Ayarlarý ---
    //public Transform[] patrolPoints;
    private Transform[] patrolPoints;
    public PatrolPath assignedPath;
    private int destPoint = 0;

    // --- D. Görüþ ve Mesafe Ayarlarý ---
    public float sightRange = 50f;
    public float attackRange = 50f;
    public float fieldOfViewAngle = 90f;
    public float eyeLevelHeight = 1.5f;

    // --- E. Saldýrý Ayarlarý ---
    public float timeBetweenAttacks = 1f;
    public int attackDamage = 4;
    private bool alreadyAttacked;

    [Header("G. Saldýrý (Ateþ Etme) Ayarlarý")]
    public GameObject bulletPrefab; // Adým 2'de oluþturduðun mermi prefab'ý
    public Transform firePoint;     // Adým 3'te oluþturduðun çýkýþ noktasý
    public float bulletSpeed = 50f; // Merminin ne kadar hýzlý gideceði

    [Header("H. Can ve Ölüm Ayarlarý")]
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    private bool isAlive = true;

    // --- F. Baþlangýç ---
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoRepath = true;
        agent.autoBraking = false;

        animator = GetComponentInChildren<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        playerTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerObj != null)
            playerTarget = playerObj.transform;

        if (assignedPath != null && assignedPath.points.Length > 0)
        {
            patrolPoints = assignedPath.points;
        }
        else
        {
            // Atanmýþ yol yoksa, boþ bir dizi oluþtur ki hata vermesin
            patrolPoints = new Transform[0];
        }
        // Baþlangýç devriye
        if (patrolPoints.Length > 0)
        {
            destPoint = 0;
            agent.SetDestination(patrolPoints[destPoint].position);
            currentState = EnemyState.Patrol;
        }
        else // Devriye yolu yoksa, IDLE baþla
        {
            currentState = EnemyState.Idle;
        }
    }

    // --- G. Güncelleme Döngüsü ---
    void Update()
    {
        if (!isAlive) return;
        if (playerTarget == null) return;

        CheckTransitions();

        // Animasyon parametrelerini güncelle
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
            animator.SetBool("IsShooting", currentState == EnemyState.Attack);
        }

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

    // --- H. GÖRÜÞ KONTROLÜ ---
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

    // --- I. DAVRANIÞLAR ---
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
        // 1. Botun hareketini durdur
        agent.SetDestination(transform.position);

        // 2. Botun BEDENÝ hala yatay olarak dönsün (bu iyi bir þey)
        Vector3 lookAtTarget = playerTarget.position;
        lookAtTarget.y = transform.position.y;
        transform.LookAt(lookAtTarget);

        if (!alreadyAttacked)
        {
            // --- MERMÝ FIRLATMA MANTIÐINI DEÐÝÞTÝRÝYORUZ ---

            // 3. Hedefin ayaklarý yerine gövdesine (veya kafasýna) niþan al
            //    (Vector3.up * 1f = hedefin pivotundan 1 metre yukarýsý)
            Vector3 targetCenter = playerTarget.position + Vector3.up * 1f;

            // 4. Namlu ucundan (firePoint) hedefin merkezine giden YÖNÜ hesapla
            Vector3 directionToTarget = (targetCenter - firePoint.position).normalized;

            // 5. Bu yöne bakan doðru ROTASYONU (Quaternion) hesapla
            Quaternion bulletRotation = Quaternion.LookRotation(directionToTarget);

            // 6. Mermiyi, "firePoint.rotation" (namlunun baktýðý yer) yerine,
            //    bizim hesapladýðýmýz "bulletRotation" (hedefe giden yön) ile oluþtur.
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, bulletRotation);

            // --- DEÐÝÞÝKLÝK BÝTTÝ ---

            // Merminin içindeki script'e ulaþ
            // (Script adýnýn EnemyBullet olduðundan emin ol)
            EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
            if (bulletScript != null)
            {
                bulletScript.damage = attackDamage;
                bulletScript.speed = bulletSpeed;
                bulletScript.shooter = this.transform;
            }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // --- J. DURUM GEÇÝÞLERÝ ---
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
    /// <summary>
    /// Düþmanýn hasar almasýný saðlar.
    /// </summary>
    public void TakeDamage(float amount)
    {
        // Eðer zaten öldüyse, tekrar hasar almasýn
        if (!isAlive) return;

        currentHealth -= amount;
        Debug.Log(gameObject.name + " " + amount + " hasar aldý. Kalan can: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Düþman öldüðünde çalýþýr.
    /// </summary>
    private void Die()
    {
        isAlive = false;
        Debug.Log(gameObject.name + " öldü.");

        // 1. Yapay zekayý ve hareketi durdur
        if (agent != null) agent.enabled = false;
        this.enabled = false; // Bu script'i (EnemyAI) devre dýþý býrak

        // 2. Animasyonu tetikle (Animator'de "Die" adýnda bir Trigger olmalý)
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 3. Çarpýþmayý kapat (ceset havada kalmasýn veya mermileri engellemesin)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // 4. (Opsiyonel) Ragdoll'u aktifleþtir (eðer varsa)

        // 5. Cesedi 5 saniye sonra yok et
        Destroy(gameObject, 0.1f);
    }
}
