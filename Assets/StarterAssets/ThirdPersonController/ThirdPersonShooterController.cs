using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private Transform leftHandIKTarget;
    [SerializeField] private Transform leftHandGripPoint;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Vector3 weaponRotationOffset;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera playerZoomCamera;
    [SerializeField] private Rig aimRig;
    private AudioSource audioSource;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform vfxHitGreen;
    [SerializeField] private Transform vfxHitRed;
    [SerializeField] private ParticleSystem muzzleFlashEffect;
    [SerializeField] private WeaponAmmo weaponAmmo;

    [Header("Ses Efektleri")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip reloadSound;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private float aimRigWeight;
    private float leftHandIKWeight;

    private Quaternion defaultLocalRotation;

    private bool isReloading = false;
    private int reloadLayerIndex;


    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        playerZoomCamera.gameObject.SetActive(false);
        

        if (weaponTransform != null)
            defaultLocalRotation = weaponTransform.localRotation;

        if (weaponAmmo != null)
        {
            weaponAmmo.shooterController = this;
        }

        reloadLayerIndex = animator.GetLayerIndex("Reload Layer");
    }

    private void Update()
    {
        // --- Aiming Ray (crosshair yönü) ---
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        Transform hitTransform = null;

        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f);

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
            targetPoint = raycastHit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000f);
            mouseWorldPosition = targetPoint;
        }

        // Ateþ etmeden niþan alma
        if (starterAssetsInputs.shoot && !starterAssetsInputs.aim)
            starterAssetsInputs.aim = true;

        // --- AIM aktifken ---
        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 13f));
            animator.SetBool("IsAiming", true);

            // Karakter sadece Y ekseninde dönsün
            Vector3 worldAimTarget = targetPoint;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 15f);

            aimRigWeight = 1f;
            leftHandIKWeight = 1f;

            // ?? Silah Rotasyonu DÜZELTÝLDÝ
            if (weaponTransform != null)
            {
                // Crosshair yönüne bak
                Vector3 aimDir = (targetPoint - spawnBulletPosition.position).normalized;

                // Silahýn namlusu -X'e bakýyorsa bunu 180° düzelt
                Quaternion modelFix = Quaternion.Euler(0, 180f, 0f);

                // Rotasyonu hesapla
                Quaternion aimRot = Quaternion.LookRotation(aimDir) * modelFix * Quaternion.Euler(weaponRotationOffset);

                // Silah parent'ýna göre local rotasyon uygula
                Quaternion localTargetRot = Quaternion.Inverse(weaponTransform.parent.rotation) * aimRot;

                weaponTransform.localRotation = Quaternion.Slerp(
                    weaponTransform.localRotation,
                    localTargetRot,
                    Time.deltaTime * 15f
                );
            }

            playerZoomCamera.gameObject.SetActive(true);

            // --- Ateþ Etme ---
            if (starterAssetsInputs.shoot)
            {
                if (!isReloading)
                {
                    if (weaponAmmo.TryFire())
                    {
                        if (muzzleFlashEffect != null)
                            muzzleFlashEffect.Play();

                        Vector3 aimDir = (targetPoint - spawnBulletPosition.position).normalized;
                        Instantiate(
                            pfBulletProjectile,
                            spawnBulletPosition.position,
                            Quaternion.LookRotation(aimDir, Vector3.up)
                        );
                        if (audioSource != null && fireSound != null)
                        {
                            audioSource.PlayOneShot(fireSound);
                        }
                    }
                    else if (weaponAmmo.CanReload())
                    {
                        StartReload();
                    }
                }
                starterAssetsInputs.shoot = false;
            }
        }
        else
        {
            // --- AIM kapalýyken ---
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 13f));
            animator.SetBool("IsAiming", false);
            playerZoomCamera.gameObject.SetActive(false);
            aimRigWeight = 0f;
            leftHandIKWeight = 0f;

            if (weaponTransform != null)
            {
                weaponTransform.localRotation = Quaternion.Lerp(
                    weaponTransform.localRotation,
                    defaultLocalRotation,
                    Time.deltaTime * 10f
                );
            }
        }

        // --- Reload kontrol ---
        if (starterAssetsInputs.reload && !isReloading && weaponAmmo.CanReload())
        {
            StartReload();
            starterAssetsInputs.reload = false;
        }

        // --- Left Hand IK ---
        if (leftHandIK != null)
        {
            leftHandIK.weight = Mathf.Lerp(leftHandIK.weight, leftHandIKWeight, Time.deltaTime * 10f);
        }
    }
    private void LateUpdate()
    {
        if (leftHandIKTarget != null && leftHandGripPoint != null)
        {
            leftHandIKTarget.position = leftHandGripPoint.position;
            leftHandIKTarget.rotation = leftHandGripPoint.rotation;
        }
    }


    // --- YENÝ FONKSÝYONLAR ---

    /// <summary>
    /// Þarjör yenileme animasyonunu ve durumunu baþlatýr.
    /// </summary>
    private void StartReload()
    {
        isReloading = true;

        animator.SetLayerWeight(reloadLayerIndex, 1f); // Reload katmanýný aktifleþtir
        animator.SetTrigger("Reload"); // Reload animasyonunu tetikle

        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
    }

    /// <summary>
    /// Animasyon Event'i tarafýndan çaðrýlacak: Þarjör deðiþtirme iþlemini yapar.
    /// </summary>
    public void OnReloadLogic()
    {
        weaponAmmo.PerformReloadLogic();
    }

    /// <summary>
    /// Animasyon Event'i tarafýndan çaðrýlacak: Animasyon bittiðinde durumu sýfýrlar.
    /// </summary>
    public void OnReloadComplete()
    {
        isReloading = false;
        animator.SetLayerWeight(reloadLayerIndex, 0f); // Reload katmanýný devre dýþý býrak
    }
}