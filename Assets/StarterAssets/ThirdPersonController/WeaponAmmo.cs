using UnityEngine;
using TMPro; // TextMeshPro UI kullanmak için bu satýr gerekli

public class WeaponAmmo : MonoBehaviour
{


    [Header("Ammo Ayarlarý")]
    public int clipSize = 30; // Þarjör kapasitesi
    public int maxTotalAmmo = 150;
    public int totalAmmo = 120; // Toplam taþýnabilen mermi
    [HideInInspector]
    public int currentAmmoInClip; // Þarjördeki anlýk mermi



    [Header("UI Referansý")]
    public TextMeshProUGUI ammoText; // Canvas'taki mermi sayýsýný gösteren text

    // Controller script'ine referans
    [HideInInspector]
    public ThirdPersonShooterController shooterController;

    private void Awake()
    {
        currentAmmoInClip = clipSize; // Oyuna tam þarjörle baþla
        UpdateAmmoUI();
    }

    /// <summary>
    /// Ateþ etmeyi dener. Mermi varsa true, yoksa false döner.
    /// </summary>
    public bool TryFire()
    {
        if (currentAmmoInClip > 0)
        {
            currentAmmoInClip--; // Mermiyi eksilt
            UpdateAmmoUI();
            return true; // Ateþ edildi
        }
        return false; // Mermi yok
    }

    /// <summary>
    /// Þarjör yenileme kontrolü.
    /// </summary>
    public bool CanReload()
    {
        // Þarjör zaten doluysa VEYA hiç mermimiz kalmadýysa (0/0) reload yapma
        if (currentAmmoInClip == clipSize || totalAmmo == 0)
        {
            return false;
        }
        return true; // Reload yapýlabilir
    }

    /// <summary>
    /// Animasyon event'i tarafýndan çaðrýlacak asýl þarjör doldurma iþlemi.
    /// </summary>
    public void PerformReloadLogic()
    {
        int ammoNeeded = clipSize - currentAmmoInClip; // Þarjöre kaç mermi lazým?
        int ammoToDeduct = Mathf.Min(ammoNeeded, totalAmmo); // Toplam mermiden ne kadar alabiliriz?

        currentAmmoInClip += ammoToDeduct; // Þarjörü doldur
        totalAmmo -= ammoToDeduct; // Toplam mermiden düþ

        UpdateAmmoUI();
    }

    /// <summary>
    /// UI Text'ini günceller (örn: "30 / 120").
    /// </summary>
    //public void UpdateAmmoUI()
    //{
    //    if (ammoText != null)
    //    {
    //        ammoText.text = $"{currentAmmoInClip} / {totalAmmo}";
    //    }
    //}

    // ... (Diðer fonksiyonlarýn altýnda)

    /// <summary>
    /// Oyuncunun daha fazla mermi alýp alamayacaðýný kontrol eder.
    /// </summary>
    public bool CanAddAmmo()
    {
        return totalAmmo < maxTotalAmmo;
    }

    /// <summary>
    /// Oyuncunun toplam mermisine mermi ekler ve UI'ý günceller.
    /// Baþarýyla eklendiyse true, mermi zaten doluysa false döner.
    /// </summary>
    /// <summary>
    /// Oyuncunun toplam mermisine mermi ekler ve UI'ý günceller.
    /// Baþarýyla eklendiyse true, mermi zaten doluysa false döner.
    /// </summary>
    public bool AddAmmo(int amount)
    {
        // --- YENÝ DEBUG ---
        Debug.Log("WeaponAmmo.cs: AddAmmo fonksiyonu çaðrýldý. Eklenecek miktar: " + amount);

        if (totalAmmo >= maxTotalAmmo)
        {
            Debug.LogWarning("WeaponAmmo.cs: AddAmmo 'false' döndürdü (Zaten mermi dolu).");
            return false; // Zaten mermi dolu, alamaz.
        }

        int eskiMermi = totalAmmo; // Debug için eski miktarý tut
        totalAmmo += amount; // Mermiyi ekle

        // Eðer ekleme sonucu maksimum limiti aþtýysak, limite eþitle
        if (totalAmmo > maxTotalAmmo)
        {
            totalAmmo = maxTotalAmmo;
        }

        // --- YENÝ DEBUG ---
        Debug.Log("WeaponAmmo.cs: Miktar eklendi. Eski Toplam Mermi: " + eskiMermi + ", Yeni Toplam Mermi: " + totalAmmo);

        UpdateAmmoUI(); // UI'ý güncelle
        return true; // Baþarýyla eklendi.
    }

    /// <summary>
    /// UI Text'ini günceller (örn: "30 / 120").
    /// </summary>
    public void UpdateAmmoUI()
    {
        // --- YENÝ DEBUG ---
        Debug.Log("WeaponAmmo.cs: UpdateAmmoUI fonksiyonu çaðrýldý.");

        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmoInClip} / {totalAmmo}";

            // --- YENÝ DEBUG ---
            Debug.Log("WeaponAmmo.cs: UI Baþarýyla Güncellendi. Yeni Deðer: " + ammoText.text);
        }
        else
        {
            // --- YENÝ DEBUG ---
            Debug.LogError("HATA: WeaponAmmo.cs script'indeki 'ammoText' referansý (UI Text) atanmamýþ (NULL)!");
        }
    }
}