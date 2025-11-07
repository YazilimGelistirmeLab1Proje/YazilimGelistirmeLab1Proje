using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(Animator))]
public class CharacterAnimator : MonoBehaviour
{
    [Header("IK and Rigs")]
    [SerializeField] private Rig aimRig;
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandIKTarget;
    [SerializeField] private Transform leftHandGripPoint;

    private Animator animator;
    private float aimRigWeight;
    private float leftHandIKWeight;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateAnimationState(bool isAiming, bool isReloading)
    {
        animator.SetBool("IsAiming", isAiming);

        if (isAiming && !isReloading)
        {
            aimRigWeight = 1f;
            leftHandIKWeight = 1f;
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 13f));
        }
        else
        {
            aimRigWeight = 0f;
            leftHandIKWeight = 0f;
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 13f));
        }

        if (aimRig != null)
        {
            aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f);
        }
        
        if (leftHandIK != null)
        {
            leftHandIK.weight = Mathf.Lerp(leftHandIK.weight, leftHandIKWeight, Time.deltaTime * 10f);
        }
    }

    public void HandleIK()
    {
        if (leftHandIKTarget != null && leftHandGripPoint != null)
        {
            leftHandIKTarget.position = leftHandGripPoint.position;
            leftHandIKTarget.rotation = leftHandGripPoint.rotation;
        }
    }
}
