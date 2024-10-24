using UnityEngine;

public class OVRControllerManager : MonoBehaviour
{
    private CharacterController characterController;
    private OVRPlayerController ovrPlayerController;

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
    }

    void Update()
    {
        if (ovrPlayerController != null && characterController != null)
        {
            HandleMovement();  // 이동 처리
            HandleRotation();  // 회전 처리
        }
    }

    private void HandleMovement()
    {
        // L 컨트롤러 (Primary Thumbstick)로만 이동
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        // 회전 입력을 무시하고 이동 벡터만 사용
        if (primaryAxis.magnitude > 0.1f)  // 작은 움직임 필터링
        {
            Vector3 moveDirection = new Vector3(primaryAxis.x, 0, primaryAxis.y);
            moveDirection = transform.TransformDirection(moveDirection) * ovrPlayerController.Acceleration;

            if (characterController.isGrounded && OVRInput.GetDown(OVRInput.Button.One))  // 점프
            {
                moveDirection.y = ovrPlayerController.JumpForce;
            }

            // 중력 적용
            moveDirection.y += Physics.gravity.y * ovrPlayerController.GravityModifier * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    float x;
    float rotSpeed = 200.0f;
    private void HandleRotation()
    {
        // R 컨트롤러 (Secondary Thumbstick)로만 회전
        Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        x += secondaryAxis.x * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, x, 0);
        //if (Mathf.Abs(secondaryAxis.x) > 0.1f)  // 작은 움직임 무시
        //{
        //    float rotationAmount = secondaryAxis.x * ovrPlayerController.RotationAmount;

        //    // 부드러운 회전 적용
        //    Quaternion targetRotation = Quaternion.Euler(0, rotationAmount, 0) * transform.rotation;
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        //}
    }
}
