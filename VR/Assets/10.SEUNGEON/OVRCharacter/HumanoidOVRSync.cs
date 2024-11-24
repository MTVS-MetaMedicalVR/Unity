using UnityEngine;

[RequireComponent(typeof(Animator))]
public class HumanoidOVRSync : MonoBehaviour
{
    private Animator animator;
    private OVRCameraRig cameraRig;
    private Transform headConstraint;
    private Transform leftHandConstraint;
    private Transform rightHandConstraint;

    [Header("References")]
    public Transform avatarRoot;
    public float heightOffset = 0f;

    [Header("Body Settings")]
    [Range(0f, 1f)] public float bodyFollowWeight = 0.5f;
    public float turnSmoothness = 5f;
    public float moveThreshold = 0.1f;

    [Header("IK Settings")]
    [Range(0f, 1f)] public float headIKWeight = 1f;
    [Range(0f, 1f)] public float handsIKWeight = 1f;
    public Vector3 handPositionOffset = Vector3.zero;
    public Vector3 handRotationOffset = Vector3.zero;

    [Header("Debug")]
    public bool showDebugGizmos = true;
    public Color debugColor = Color.green;

    private Vector3 lastHeadPosition;
    private bool isInitialized = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        cameraRig = FindObjectOfType<OVRCameraRig>();

        if (cameraRig == null)
        {
            Debug.LogError("OVRCameraRig not found in scene!");
            return;
        }

        InitializeConstraints();
        InitializeAvatarPosition();
        isInitialized = true;
    }

    private void InitializeConstraints()
    {
        // Create empty GameObjects as constraints
        headConstraint = CreateConstraint("HeadConstraint");
        leftHandConstraint = CreateConstraint("LeftHandConstraint");
        rightHandConstraint = CreateConstraint("RightHandConstraint");
    }

    private Transform CreateConstraint(string name)
    {
        GameObject constraint = new GameObject(name);
        constraint.transform.parent = transform;
        return constraint.transform;
    }

    private void InitializeAvatarPosition()
    {
        if (avatarRoot == null) avatarRoot = transform;

        // Set initial position
        Vector3 headPosition = cameraRig.centerEyeAnchor.position;
        Vector3 feetPosition = new Vector3(headPosition.x, transform.position.y, headPosition.z);
        avatarRoot.position = feetPosition;
        lastHeadPosition = headPosition;

        // Set initial rotation
        Vector3 forward = cameraRig.centerEyeAnchor.forward;
        forward.y = 0;
        avatarRoot.rotation = Quaternion.LookRotation(forward);
    }

    private void LateUpdate()
    {
        if (!isInitialized) return;

        UpdateHeadConstraint();
        UpdateHandConstraints();
        UpdateBodyPosition();
        UpdateBodyRotation();
    }

    private void UpdateHeadConstraint()
    {
        headConstraint.position = cameraRig.centerEyeAnchor.position;
        headConstraint.rotation = cameraRig.centerEyeAnchor.rotation;
    }

    private void UpdateHandConstraints()
    {
        // Left Hand
        leftHandConstraint.position = cameraRig.leftHandAnchor.position +
            cameraRig.leftHandAnchor.TransformVector(handPositionOffset);
        leftHandConstraint.rotation = cameraRig.leftHandAnchor.rotation *
            Quaternion.Euler(handRotationOffset);

        // Right Hand
        rightHandConstraint.position = cameraRig.rightHandAnchor.position +
            cameraRig.rightHandAnchor.TransformVector(handPositionOffset);
        rightHandConstraint.rotation = cameraRig.rightHandAnchor.rotation *
            Quaternion.Euler(handRotationOffset);
    }

    private void UpdateBodyPosition()
    {
        Vector3 headMovement = cameraRig.centerEyeAnchor.position - lastHeadPosition;
        headMovement.y = 0; // Only consider horizontal movement

        if (headMovement.magnitude > moveThreshold)
        {
            Vector3 targetPosition = avatarRoot.position + headMovement;
            avatarRoot.position = Vector3.Lerp(
                avatarRoot.position,
                targetPosition,
                bodyFollowWeight
            );
        }

        // Update avatar height
        float targetHeight = cameraRig.centerEyeAnchor.position.y -
            animator.GetBoneTransform(HumanBodyBones.Head).position.y +
            heightOffset;

        Vector3 newPosition = avatarRoot.position;
        newPosition.y = targetHeight;
        avatarRoot.position = newPosition;

        lastHeadPosition = cameraRig.centerEyeAnchor.position;
    }

    private void UpdateBodyRotation()
    {
        Vector3 headForward = cameraRig.centerEyeAnchor.forward;
        headForward.y = 0;

        if (headForward.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(headForward);
            avatarRoot.rotation = Quaternion.Lerp(
                avatarRoot.rotation,
                targetRotation,
                Time.deltaTime * turnSmoothness
            );
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!isInitialized || animator == null) return;

        // Head IK
        animator.SetLookAtWeight(headIKWeight);
        animator.SetLookAtPosition(headConstraint.position);

        // Hands IK
        // Left Hand
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, handsIKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, handsIKWeight);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandConstraint.position);
        animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandConstraint.rotation);

        // Right Hand
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, handsIKWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, handsIKWeight);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandConstraint.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandConstraint.rotation);
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos || !isInitialized) return;

        Gizmos.color = debugColor;

        // Draw head constraint
        Gizmos.DrawWireSphere(headConstraint.position, 0.1f);
        DrawAxisGizmo(headConstraint, 0.2f);

        // Draw hand constraints
        Gizmos.DrawWireSphere(leftHandConstraint.position, 0.05f);
        DrawAxisGizmo(leftHandConstraint, 0.1f);

        Gizmos.DrawWireSphere(rightHandConstraint.position, 0.05f);
        DrawAxisGizmo(rightHandConstraint, 0.1f);
    }

    private void DrawAxisGizmo(Transform transform, float size)
    {
        // Forward (Z) - Blue
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * size);

        // Right (X) - Red
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * size);

        // Up (Y) - Green
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.up * size);
    }

    // Optional: Animation Events
    private void OnFootstep()
    {
        // Add footstep sound or effects here
    }

    // Optional: Physical movement simulation
    public void ApplyMovementAnimation()
    {
        if (!isInitialized || animator == null) return;

        Vector3 horizontalVelocity = (cameraRig.centerEyeAnchor.position - lastHeadPosition) / Time.deltaTime;
        horizontalVelocity.y = 0;

        float speed = horizontalVelocity.magnitude;
        Vector3 normalizedVelocity = horizontalVelocity.normalized;

        // Convert world space velocity to local space
        Vector3 localVelocity = transform.InverseTransformDirection(normalizedVelocity);

        // Update animator parameters if you have them
        animator.SetFloat("Speed", speed);
        animator.SetFloat("HorizontalSpeed", localVelocity.x);
        animator.SetFloat("VerticalSpeed", localVelocity.z);
    }
}