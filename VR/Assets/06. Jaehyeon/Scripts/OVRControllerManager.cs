using UnityEngine;
using Fusion;

public class OVRControllerManager : MonoBehaviour
{
    public Transform cameraRig;  // OVR Camera Rig ������
    private CharacterController characterController;
    private OVRPlayerController ovrPlayerController;

    float currentYRotation;
    float targetYRotation;
    float rotSpeed = 100.0f;  // ȸ�� �ӵ�
    float smoothTime = 0.05f;  // �ε巯�� ȸ�� �ð�
    float rotationVelocity;   // ȸ�� �ӵ� ������ ����

    void Start()
    {
        // CharacterController�� OVRPlayerController�� �������ų� �߰�
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            Debug.Log("CharacterController�� �ڵ����� �߰��߽��ϴ�.");
        }

        ovrPlayerController = GetComponent<OVRPlayerController>();
        if (ovrPlayerController == null)
        {
            ovrPlayerController = gameObject.AddComponent<OVRPlayerController>();
            Debug.Log("OVRPlayerController�� �ڵ����� �߰��߽��ϴ�.");
        }

        if (cameraRig == null)
        {
            //Debug.LogError("cameraRig�� �Ҵ���� �ʾҽ��ϴ�. Unity �����Ϳ��� �����ϼ���.");
        }
    }

    void Update()
    {
        if (ovrPlayerController != null && characterController != null && cameraRig != null)
        {
            HandleMovement();  // �̵� ó��
            HandleRotation();  // ȸ�� ó��
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

        // ��ǥ ȸ�� �� ����
        targetYRotation += secondaryAxis.x * rotSpeed * Time.deltaTime;

        // �ε巴�� ȸ��: ���� ȸ���� ��ǥ ȸ���� ����
        currentYRotation = Mathf.SmoothDampAngle(
            currentYRotation,
            targetYRotation,
            ref rotationVelocity,
            smoothTime
        );

        // ȸ�� ����
        cameraRig.eulerAngles = new Vector3(0, currentYRotation, 0);
    }
}
