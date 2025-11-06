using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Ayarlar")]
    public int ammoAmount = 30;

    [Header("Efektler (Opsiyonel)")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        // --- HATA AYIKLAMA 1 ---
        Debug.Log(gameObject.name + " trigger'ýna çarpan obje: " + other.gameObject.name);

        // Player objesinden WeaponAmmo script'ini bulmayý dene (GÜNCELLENDÝ)
        // Sadece çarpan objeye deðil, onun parent'larýna da bak.
        WeaponAmmo weaponAmmo = other.GetComponentInParent<WeaponAmmo>();

        // Eðer script'i (çarpan objede veya parent'ýnda) bulabildiysek
        if (weaponAmmo != null)
        {
            // --- HATA AYIKLAMA 2 ---
            // Tag kontrolünü burada yapmak daha mantýklý
            // WeaponAmmo script'ini bulduðumuz objenin tag'i "Player" mý?
            if (weaponAmmo.CompareTag("Player"))
            {
                Debug.Log("Çarpan obje veya parent'ý 'Player' olarak etiketlenmiþ ve 'WeaponAmmo' script'i bulundu.");

                if (weaponAmmo.CanAddAmmo())
                {
                    // ... (Geri kalan kodun hepsi ayný) ...
                    Debug.Log("Mermi eklenebilir...");

                    bool ammoAdded = weaponAmmo.AddAmmo(ammoAmount);

                    if (ammoAdded)
                    {
                        Debug.Log("Mermi baþarýyla EKLENDÝ.");
                        // ... (Efektler ve Destroy) ...
                        Destroy(gameObject);
                    }
                    else
                    {
                        Debug.LogWarning("AddAmmo() 'false' döndürdü. Mermi dolu.");
                    }
                }
                else
                {
                    Debug.LogWarning("CanAddAmmo() 'false' döndürdü. Mermi dolu.");
                }
            }
            else
            {
                // --- HATA AYIKLAMA 8 ---
                Debug.LogWarning("WeaponAmmo script'i bulundu AMA script'in bulunduðu objenin Tag'i 'Player' DEÐÝL!");
            }
        }
        else
        {
            // --- HATA AYIKLAMA 3 (Artýk daha doðru) ---
            Debug.LogError("Çarpan objede veya onun parent'larýnda 'WeaponAmmo' script'i BULUNAMADI!");
        }
    }
}