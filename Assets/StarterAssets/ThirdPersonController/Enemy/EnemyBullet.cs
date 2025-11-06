using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    // Bu de�erleri AI script'i dolduracak
    public int damage = 10;
    public float speed = 150f;
    public string targetTag = "Player"; // Merminin kime hasar verece�i

    // Mermiyi f�rlatan� (botu) tutar, kendine �arpmas�n diye
    public Transform shooter;

    // Ba�lang��ta mermiye h�z ver
    void Start()
    {
        // Mermiyi kendi ileri y�n�nde (Z ekseni) f�rlat
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
        }

        // Mermi 5 saniye sonra bir �eye �arpmazsa kendini yok et
        Destroy(gameObject, 5f);
    }

    // Bir �eye �arpt���nda
    void OnTriggerEnter(Collider other)
    {
        // Kendi f�rlatan�na (bot'a) �arparsa g�rmezden gel
        if (other.transform == shooter)
        {
            return;
        }

        // Hedefimiz (Player) mi?
        if (other.CompareTag(targetTag))
        {
            PlayerStats playerHealth = other.GetComponent<PlayerStats>();
            if (playerHealth != null)
            {
                // Hasar ver
                playerHealth.TakeDamage(damage);
                Debug.Log("Mermi oyuncuya �arpt�, " + damage + " hasar verdi.");
            }
        }

        // (Opsiyonel: E�er merminin duvarlara de�il, sadece oyuncuya �arp�nca yok olmas�n� istersen
        // a�a��daki sat�r� "if (other.CompareTag(targetTag))" blo�unun i�ine ta��yabilirsin.)

        // Oyuncuya veya ba�ka bir �eye (duvar vb.) �arpt���nda kendini yok et
        Destroy(gameObject);
    }
}