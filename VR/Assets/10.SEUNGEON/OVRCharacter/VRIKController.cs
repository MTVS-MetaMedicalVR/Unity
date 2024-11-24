using UnityEngine;

public class VRIKController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private OVRCameraRig cameraRig;
    [SerializeField] private Animator animator;

    [Header("Avatar Parts")]
    [SerializeField] private Transform avatarHead;
    [SerializeField] private Transform avatarLeftHand;
    [SerializeField] private Transform avatarRightHand;
    [SerializeField] private Transform avatarHips;
    [SerializeField] private Transform avatarLeftFoot;
    [SerializeField] private Transform avatarRightFoot;

    [Header("IK Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float footOffset = 0.1f;
    [SerializeField] private float raycastDistance = 1.5f;
    [SerializeField] private float smoothness = 10f;

    private Vector3 leftFootPosition;
    private Vector3 rightFootPosition;
    private Quaternion leftFootRotation;
    private Quaternion rightFootRotation;
    private float leftFootWeight;
    private float rightFootWeight;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // �ʱ� �� ��ġ ����
        leftFootPosition = avatarLeftFoot.position;
        rightFootPosition = avatarRightFoot.position;
        leftFootRotation = avatarLeftFoot.rotation;
        rightFootRotation = avatarRightFoot.rotation;
    }

    private void Update()
    {
        UpdateBodyPosition();
        UpdateFootIK();
    }

    private void UpdateBodyPosition()
    {
        // �Ӹ� ��ġ�� ������� ������ ��ġ ���
        if (avatarHips != null)
        {
            Vector3 headPosition = cameraRig.centerEyeAnchor.position;
            avatarHips.position = new Vector3(headPosition.x, transform.position.y, headPosition.z);

            // ��ü ȸ�� ����
            Vector3 forward = Vector3.ProjectOnPlane(cameraRig.centerEyeAnchor.forward, Vector3.up).normalized;
            avatarHips.rotation = Quaternion.LookRotation(forward);
        }

        // �Ӹ� ����ȭ
        if (avatarHead != null)
        {
            avatarHead.position = cameraRig.centerEyeAnchor.position;
            avatarHead.rotation = cameraRig.centerEyeAnchor.rotation;
        }

        // �� ����ȭ
        if (avatarLeftHand != null)
        {
            avatarLeftHand.position = cameraRig.leftHandAnchor.position;
            avatarLeftHand.rotation = cameraRig.leftHandAnchor.rotation;
        }

        if (avatarRightHand != null)
        {
            avatarRightHand.position = cameraRig.rightHandAnchor.position;
            avatarRightHand.rotation = cameraRig.rightHandAnchor.rotation;
        }
    }

    private void UpdateFootIK()
    {
        // �޹� IK
        RaycastHit leftHit;
        if (Physics.Raycast(avatarLeftFoot.position + Vector3.up, Vector3.down, out leftHit, raycastDistance, groundLayer))
        {
            leftFootPosition = leftHit.point + Vector3.up * footOffset;
            leftFootRotation = Quaternion.FromToRotation(Vector3.up, leftHit.normal) * avatarLeftFoot.rotation;
            leftFootWeight = 1f;
        }
        else
        {
            leftFootWeight = 0f;
        }

        // ������ IK
        RaycastHit rightHit;
        if (Physics.Raycast(avatarRightFoot.position + Vector3.up, Vector3.down, out rightHit, raycastDistance, groundLayer))
        {
            rightFootPosition = rightHit.point + Vector3.up * footOffset;
            rightFootRotation = Quaternion.FromToRotation(Vector3.up, rightHit.normal) * avatarRightFoot.rotation;
            rightFootWeight = 1f;
        }
        else
        {
            rightFootWeight = 0f;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        // �� IK ����
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPosition);
        animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRotation);

        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPosition);
        animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootRotation);

        // �� IK ����
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, avatarLeftHand.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, avatarLeftHand.rotation);

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        animator.SetIKPosition(AvatarIKGoal.RightHand, avatarRightHand.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, avatarRightHand.rotation);
    }
}