using UnityEngine;
using StarterAssets;

[RequireComponent(typeof(StarterAssetsInputs))]
[RequireComponent(typeof(ThirdPersonController))]
[RequireComponent(typeof(PlayerAiming))]
[RequireComponent(typeof(WeaponManager))]
[RequireComponent(typeof(CharacterAnimator))]
public class ThirdPersonShooterController : MonoBehaviour
{
    private StarterAssetsInputs starterAssetsInputs;
    private PlayerAiming playerAiming;
    private WeaponManager weaponManager;
    private CharacterAnimator characterAnimator;

    private void Awake()
    {
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        playerAiming = GetComponent<PlayerAiming>();
        weaponManager = GetComponent<WeaponManager>();
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    private void Update()
    {
        playerAiming.HandleAiming(starterAssetsInputs.aim);
        characterAnimator.UpdateAnimationState(playerAiming.IsAiming, weaponManager.IsReloading);
        HandleShootingInput();
        HandleReloadInput();
    }

    private void HandleShootingInput()
    {
        if (starterAssetsInputs.shoot && !starterAssetsInputs.aim)
        {
            starterAssetsInputs.aim = true;
            playerAiming.HandleAiming(true); 
        }

        if (starterAssetsInputs.shoot)
        {
            weaponManager.TryFire(playerAiming.AimTarget);
            starterAssetsInputs.shoot = false;
        }
    }

    private void HandleReloadInput()
    {
        if (starterAssetsInputs.reload)
        {
            weaponManager.TryReload();
            starterAssetsInputs.reload = false;
        }
    }

    private void LateUpdate()
    {
        characterAnimator.HandleIK();
    }
}
