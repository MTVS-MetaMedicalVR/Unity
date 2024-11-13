using UnityEngine;
using Fusion;

public class OVRControllerManager : MonoBehaviour
{
    public Transform cameraRig;  // OVR Camera Rig 참조용
    private CharacterController characterController;
    private OVRPlayerController ovrPlayerController;

    float currentYRotation;
    float targetYRotation;
    float rotSpeed = 100.0f;  // 회전 속도
    float smoothTime = 0.05f;  // 부드러운 회전 시간
    float rotationVelocity;   // 회전 속도 조절용 변수

    void Start()
    {
        // CharacterController와 OVRPlayerController를 가져오거나 추가
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            Debug.Log("CharacterController를 자동으로 추가했습니다.");
        }

        ovrPlayerController = GetComponent<OVRPlayerController>();
        if (ovrPlayerController == null)
        {
            ovrPlayerController = gameObject.AddComponent<OVRPlayerController>();
            Debug.Log("OVRPlayerController를 자동으로 추가했습니다.");
        }

        if (cameraRig == null)
        {
            //Debug.LogError("cameraRig가 할당되지 않았습니다. Unity 에디터에서 설정하세요.");
        }
    }

    void Update()
    {
        if (ovrPlayerController != null && characterController != null && cameraRig != null)
        {
            HandleMovement();  // 이동 처리
            HandleRotation();  // 회전 처리
        }
    }

    private void HandleMovement()
    {
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        if (primaryAxis.magnitude > 0.1f)
        {
            Vector3 moveDirection = new Vector3(primaryAxis.x, 0, primaryAxis.y);
            moveDirection = cameraRig.TransformDirection(moveDirection) * ovrPlayerController.Acceleration;

            if (characterController.isGrounded && OVRInput.GetDown(OVRInput.Button.One))
            {
                moveDirection.y = ovrPlayerController.JumpForce;
            }

            moveDirection.y += Physics.gravity.y * ovrPlayerController.GravityModifier * Time.deltaTime;
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        // 목표 회전 값 설정
        targetYRotation += secondaryAxis.x * rotSpeed * Time.deltaTime;

        // 부드럽게 회전: 현재 회전과 목표 회전을 보간
        currentYRotation = Mathf.SmoothDampAngle(
            currentYRotation,
            targetYRotation,
            ref rotationVelocity,
            smoothTime
        );

        // 회전 적용
        cameraRig.eulerAngles = new Vector3(0, currentYRotation, 0);
    }
}
