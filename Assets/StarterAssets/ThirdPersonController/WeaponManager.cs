using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerAiming))]
public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Components")]
    [SerializeField] private WeaponAmmo weaponAmmo;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;

    [Header("Effects")]
    [SerializeField] private ParticleSystem muzzleFlashEffect;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip reloadSound;
    
    [Header("Weapon Transform")]
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Vector3 weaponRotationOffset;
    
    private AudioSource audioSource;
    private Animator animator;
    private PlayerAiming playerAiming;
    
    private Quaternion defaultLocalRotation;
    private int reloadLayerIndex;
    
    public bool IsReloading { get; private set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        playerAiming = GetComponent<PlayerAiming>();

        if (weaponTransform != null)
            defaultLocalRotation = weaponTransform.localRotation;

        if (weaponAmmo != null && weaponAmmo.shooterController == null)
        {
             // Orijinal koddaki referansý korumak için, 
             // WeaponAmmo'nun bu referansa ihtiyacý olabilir.
            weaponAmmo.shooterController = this.GetComponent<ThirdPersonShooterController>();
        }

        reloadLayerIndex = animator.GetLayerIndex("Reload Layer");
    }
    
    private void Update()
    {
        UpdateWeaponRotation(playerAiming.IsAiming, playerAiming.AimTarget);
    }

    public void TryFire(Vector3 aimTarget)
    {
        if (IsReloading) return;

        if (weaponAmmo.TryFire())
        {
            if (muzzleFlashEffect != null)
                muzzleFlashEffect.Play();

            if (audioSource != null && fireSound != null)
                audioSource.PlayOneShot(fireSound);

            Vector3 aimDir = (aimTarget - spawnBulletPosition.position).normalized;
            Instantiate(
                pfBulletProjectile,
                spawnBulletPosition.position,
                Quaternion.LookRotation(aimDir, Vector3.up)
            );
        }
        else if (weaponAmmo.CanReload())
        {
            StartReload();
        }
    }

    public void TryReload()
    {
        if (!IsReloading && weaponAmmo.CanReload())
        {
            StartReload();
        }
    }

    private void StartReload()
    {
        IsReloading = true;
        animator.SetLayerWeight(reloadLayerIndex, 1f);
        animator.SetTrigger("Reload");

        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
    }
    
    private void UpdateWeaponRotation(bool isAiming, Vector3 aimTarget)
    {
        if (weaponTransform == null) return;
        
        if (isAiming)
        {
            Vector3 aimDir = (aimTarget - aimTarget).normalized;
            if (spawnBulletPosition != null)
            {
                aimDir = (aimTarget - spawnBulletPosition.position).normalized;
            }

            Quaternion modelFix = Quaternion.Euler(0, 180f, 0f);
            Quaternion aimRot = Quaternion.LookRotation(aimDir) * modelFix * Quaternion.Euler(weaponRotationOffset);
            Quaternion localTargetRot = Quaternion.Inverse(weaponTransform.parent.rotation) * aimRot;

            weaponTransform.localRotation = Quaternion.Slerp(
                weaponTransform.localRotation,
                localTargetRot,
                Time.deltaTime * 15f
            );
        }
        else
        {
            weaponTransform.localRotation = Quaternion.Slerp(
                weaponTransform.localRotation,
                defaultLocalRotation,
                Time.deltaTime * 10f
            );
        }
    }

    public void OnReloadLogic()
    {
        weaponAmmo.PerformReloadLogic();
    }

    public void OnReloadComplete()
    {
        IsReloading = false;
        animator.SetLayerWeight(reloadLayerIndex, 0f);
    }
}
