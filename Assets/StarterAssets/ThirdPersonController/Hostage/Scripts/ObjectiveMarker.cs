using UnityEngine;
using UnityEngine.UI; // UI elemanlarý için bu kütüphane þart

public class ObjectiveMarker : MonoBehaviour
{
    // 1. Takip edilecek 3D hedef (Rehinenin kendisi)
    public Transform target;

    // 2. Rehinenin script'i (ne zaman kurtarýldýðýný bilmek için)
    public HostageController hostage;

    // 3. Ekran kenarýndan ne kadar içeride dursun
    public float margin = 30f;

    private Camera mainCamera;
    private RectTransform markerRectTransform;

    void Start()
    {
        mainCamera = Camera.main;
        markerRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Eðer rehine atanmamýþsa veya rehine ZATEN kurtarýldýysa,
        // bu ikonu gizle ve hiçbir þey yapma.
        if (target == null || hostage == null || hostage.isRescued)
        {
            markerRectTransform.gameObject.SetActive(false);
            return;
        }

        // Rehine henüz kurtarýlmadýysa, ikonu göster
        markerRectTransform.gameObject.SetActive(true);

        // 1. Hedefin 3D pozisyonunu 2D ekran pozisyonuna çevir
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        // 2. Hedef ekranýn GÖRÜNÜR alanýnda mý? (ve kameranýn önünde mi?)
        if (screenPos.z > 0 &&
            screenPos.x > margin && screenPos.x < Screen.width - margin &&
            screenPos.y > margin && screenPos.y < Screen.height - margin)
        {
            // Evet, ekranda. Ýkonu tam üstüne koy.
            markerRectTransform.position = screenPos;

            // Eðer ekran dýþýndayken ok gibi döndürdüysek, düzelt
            markerRectTransform.rotation = Quaternion.identity;
        }
        else
        {
            // Hayýr, ekranýn DIÞINDA (veya arkasýnda).

            // Eðer arkasýndaysa (z < 0), ekran koordinatlarý ters döner.
            // Bunu düzeltmek için pozisyonu ters çeviriyoruz ki ekranýn doðru kenarýna gitsin.
            if (screenPos.z < 0)
            {
                screenPos *= -1;
            }

            // Ekranýn merkez noktasýný bul
            Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);

            // Merkezden ekran dýþýndaki hedefe doðru yön vektörü
            Vector2 direction = new Vector2(screenPos.x, screenPos.y) - center;

            // Ýkonun/Ok'un hedefe doðru dönmesini saðla
            // (Eðer ikonun bir 'ok' ise ve üst tarafý hedefi gösteriyorsa -90f gerekebilir)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            markerRectTransform.rotation = Quaternion.Euler(0, 0, angle);

            // Ýkonu ekran kenarlarýna "kýstýr" (clamp)
            // (Bu basit 'clamp' köþelere yapýþmasýna neden olabilir ama en kolay yöntem budur)
            float clampedX = Mathf.Clamp(screenPos.x, margin, Screen.width - margin);
            float clampedY = Mathf.Clamp(screenPos.y, margin, Screen.height - margin);

            markerRectTransform.position = new Vector3(clampedX, clampedY, 0);
        }
    }
}