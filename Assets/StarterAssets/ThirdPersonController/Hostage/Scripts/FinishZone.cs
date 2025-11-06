using UnityEngine;
using UnityEngine.SceneManagement; // Sahne deðiþtirmek için (opsiyonel)

public class FinishZone : MonoBehaviour
{
    public GameManager gameManager;
    private int safeHostageCount = 0;
    private int requiredHostages = 2; // Oyunu bitirmek için gereken rehine sayýsý

    private void OnTriggerEnter(Collider other)
    {
        // Bölgeye giren þeyin "Hostage" tag'i var mý?
        if (other.CompareTag("Hostage"))
        {
            // Giren rehinenin script'ine ulaþ
            HostageController hostage = other.GetComponent<HostageController>();

            // Eðer rehine script'i varsa VE henüz 'güvende' olarak iþaretlenmemiþse
            if (hostage != null && !hostage.isSafe)
            {
                // Rehineyi 'güvende' olarak iþaretle (script'indeki fonksiyonu çaðýr)
                hostage.MarkAsSafe();

                // Güvendeki rehine sayýsýný artýr
                safeHostageCount++;

                Debug.Log("Güvendeki rehine sayýsý: " + safeHostageCount);

                // Gerekli sayýya ulaþtýk mý?
                CheckForWinCondition();
            }
        }
    }

    private void CheckForWinCondition()
    {
        if (safeHostageCount >= requiredHostages)
        {
            // OYUN BÝTTÝ!
            

            if (gameManager != null)
            {
                Debug.Log("GÖREV BAÞARILI! Tüm rehineler kurtarýldý!");
                gameManager.CompleteLevel(); // GameManager'daki kazanma fonksiyonunu çaðýr!
            }
            else
            {
                Debug.LogError("FinishZone script'ine GameManager objesini atamayý unuttun!");
            }

            
        }
    }
}