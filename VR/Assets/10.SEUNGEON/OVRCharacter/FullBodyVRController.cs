using UnityEngine;

[System.Serializable]
public class IKFootSolver
{
    public Transform foot;
    public Transform hint; // 무릎 힌트 포인트
    public float heightFromGround = 0.1f;
    public float raycastDistance = 1.5f;
    public Vector3 footOffset;
    public float stepSpeed = 4f;

    private Vector3 currentPosition;
    private Vector3 lastPosition;
    private Quaternion currentRotation;
    private float lerp;

    public void Init(Vector3 pos)
    {
        currentPosition = pos;
        lastPosition = pos;
        lerp = 1f;
    }

    public void Solve(LayerMask groundLayer)
    {
        Ray ray = new Ray(foot.position + Vector3.up, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance, groundLayer))
        {
            // 발이 땅에 닿았을 때의 처리
            if (lerp >= 1f)
            {
                lastPosition = currentPosition;
                currentPosition = hit.point + Vector3.up * heightFromGround;
                currentRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                lerp = 0f;
            }
        }

        lerp += Time.deltaTime * stepSpeed;
        foot.position = Vector3.Lerp(lastPosition, currentPosition, lerp) + footOffset;
        foot.rotation = currentRotation;
    }
}

[System.Serializable]
public class IKBodySolver
{
    public Transform pelvis;
    public float heightOffset = 0f;
    public float smoothSpeed = 5f;

    private Vector3 lastPosition;

    public void Solve(Vector3 targetPosition)
    {
        Vector3 targetPos = new Vector3(targetPosition.x, targetPosition.y + heightOffset, targetPosition.z);
        pelvis.position = Vector3.Lerp(lastPosition, targetPos, Time.deltaTime * smoothSpeed);
        lastPosition = pelvis.position;
    }
}
public class FullBodyVRController : MonoBehaviour
{
    [Header("VR References")]
    [SerializeField] private OVRCameraRig cameraRig;
    [SerializeField] private Animator animator;

    [Header("IK Settings")]
    [SerializeField] private IKFootSolver leftFoot = new IKFootSolver();
    [SerializeField] private IKFootSolver rightFoot = new IKFootSolver();
    [SerializeField] private IKBodySolver bodySolver = new IKBodySolver();
    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        // IK 초기화
        leftFoot.Init(leftFoot.foot.position);
        rightFoot.Init(rightFoot.foot.position);
    }

    private void Update()
    {
        // 발 IK 업데이트
        leftFoot.Solve(groundLayer);
        rightFoot.Solve(groundLayer);

        // 몸통 위치 업데이트
        bodySolver.Solve(cameraRig.centerEyeAnchor.position);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        // 손 IK
        SetHandIK(AvatarIKGoal.LeftHand, cameraRig.leftHandAnchor);
        SetHandIK(AvatarIKGoal.RightHand, cameraRig.rightHandAnchor);

        // 발 IK
        SetFootIK(AvatarIKGoal.LeftFoot, leftFoot.foot);
        SetFootIK(AvatarIKGoal.RightFoot, rightFoot.foot);
    }

    private void SetHandIK(AvatarIKGoal goal, Transform target)
    {
        animator.SetIKPositionWeight(goal, 1);
        animator.SetIKRotationWeight(goal, 1);
        animator.SetIKPosition(goal, target.position);
        animator.SetIKRotation(goal, target.rotation);
    }

    private void SetFootIK(AvatarIKGoal goal, Transform target)
    {
        animator.SetIKPositionWeight(goal, 1);
        animator.SetIKRotationWeight(goal, 1);
        animator.SetIKPosition(goal, target.position);
        animator.SetIKRotation(goal, target.rotation);
    }
}