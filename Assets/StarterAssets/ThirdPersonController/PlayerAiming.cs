using UnityEngine;
using Cinemachine;
using StarterAssets;

[RequireComponent(typeof(ThirdPersonController))]
public class PlayerAiming : MonoBehaviour
{
    [Header("Camera and Sensitivity")]
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private CinemachineVirtualCamera playerZoomCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;

    [Header("Aiming Logic")]
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    
    private ThirdPersonController thirdPersonController;

    public bool IsAiming { get; private set; }
    public Vector3 AimTarget { get; private set; }

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        
        aimVirtualCamera.gameObject.SetActive(false);
        playerZoomCamera.gameObject.SetActive(false);
    }

    public void HandleAiming(bool aimInput)
    {
        IsAiming = aimInput;
        UpdateAimTarget();

        aimVirtualCamera.gameObject.SetActive(IsAiming);
        playerZoomCamera.gameObject.SetActive(IsAiming);
        
        if (IsAiming)
        {
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            RotateCharacterTowardsAim();
        }
        else
        {
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
        }
    }

    private void UpdateAimTarget()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            AimTarget = raycastHit.point;
            if (debugTransform != null)
                debugTransform.position = raycastHit.point;
        }
        else
        {
            AimTarget = ray.GetPoint(1000f);
            if (debugTransform != null)
                debugTransform.position = AimTarget;
        }
    }

    private void RotateCharacterTowardsAim()
    {
        Vector3 worldAimTarget = AimTarget;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 15f);
    }
}
