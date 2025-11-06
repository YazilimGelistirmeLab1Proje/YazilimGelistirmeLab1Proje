using UnityEngine;
using UnityEngine.AI; 


[RequireComponent(typeof(NavMeshAgent))] 
public class HostageController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform playerTransform;
    private Animator anim; 

    public bool isRescued = false; 
    public bool isSafe = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // --- YENÝ DEBUG KODU ---
        anim = GetComponentInChildren<Animator>();

        // 1. Adým: Hiç Animator bulabildi mi?
        if (anim == null)
        {
            Debug.LogError(">>> KESÝN HATA: '" + gameObject.name + "' objesi ve çocuklarýnda HÝÇ Animator bileþeni BULUNAMADI! Script durduruluyor.", this.gameObject);
            Debug.Break(); // Oyunu sahnede durdur
            this.enabled = false; // Bu script'i devre dýþý býrak
            return;
        }

        // 2. Adým: Bulduðu Animator'ün Controller slotu dolu mu?
        if (anim.runtimeAnimatorController == null)
        {
            Debug.LogError(">>> KESÝN HATA: Script, Animator'ü '" + anim.gameObject.name + "' objesinde buldu ANCAK BU OBJENÝN 'Controller' SLOTU BOÞ! Lütfen bu objeyi seçip slotu doldurun.", anim.gameObject);
            Debug.Break(); // Oyunu sahnede durdur
            this.enabled = false; // Bu script'i devre dýþý býrak
            return;
        }

        // 3. Adým: Sorun yoksa
        Debug.Log("Animator baþarýyla '" + anim.gameObject.name + "' objesinde bulundu ve Controller'ý dolu: " + anim.runtimeAnimatorController.name, anim.gameObject);
        // --- DEBUG KODU SONU ---


        // Kodunun geri kalaný
        agent.isStopped = true;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
    }

    void Update()
    {
        
        if (isRescued && !isSafe)
        {
            
            agent.SetDestination(playerTransform.position);

            
            float currentSpeed = agent.velocity.magnitude;

            
            anim.SetFloat("Speed", currentSpeed);
        }
        else
        {
            
            anim.SetFloat("Speed", 0f);
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player") && !isRescued)
        {
            Debug.Log("Rehine kurtarýldý, oyuncuyu takip ediyor!");
            isRescued = true;
            agent.isStopped = false; 
        }
    }

   
    public void MarkAsSafe()
    {
        isSafe = true;
        agent.Stop();
        Debug.Log("Rehine kurtarma noktasýna ulaþtý!");
        
    }
}