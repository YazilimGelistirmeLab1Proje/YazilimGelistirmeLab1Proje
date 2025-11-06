// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Sahne yönetimi için bu kütüphane þart!

public class GameManager : MonoBehaviour
{
    // Unity editöründen buraya o gizlediðimiz paneli sürükleyeceksin
    public GameObject winScreenUI;

    // Bu fonksiyonu kazandýðýmýz zaman çaðýracaðýz
    public void CompleteLevel()
    {
        // 1. Kazanma ekranýný görünür yap
        if (winScreenUI != null)
        {
            winScreenUI.SetActive(true);
        }

        // 2. Oyunu dondur (Tüm fiziksel hareketler, animasyonlar durur)
        Time.timeScale = 0f;

        // 3. Fare imlecini serbest býrak ve görünür yap
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Bu fonksiyonu "Tekrar Oyna" butonuna baðlayacaðýz
    public void RestartGame()
    {
        // 1. Oyunu tekrar normal hýzýna döndür (Sahne yüklenmeden önce þart!)
        Time.timeScale = 1f;

        // 2. Aktif olan sahneyi (yani bu bölümü) yeniden yükle
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}