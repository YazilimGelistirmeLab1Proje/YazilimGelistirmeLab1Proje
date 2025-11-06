using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Düþmanýn çaðýracaðý hasar alma fonksiyonu
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Oyuncu Hasar Aldý! Kalan Can: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Oyuncu öldü! Görev baþarýsýz!");
        // *Buraya görev baþarýsýz ekraný yükleme kodu gelebilir.*
        gameObject.SetActive(false); // Oyuncuyu sahnede devre dýþý býrak
    }
}