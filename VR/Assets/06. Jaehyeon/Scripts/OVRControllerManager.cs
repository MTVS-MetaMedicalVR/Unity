using UnityEngine;

public class OVRControllerManager : MonoBehaviour
{
    private CharacterController characterController;
    private OVRPlayerController ovrPlayerController;

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
    }

    void Update()
    {
        if (ovrPlayerController != null && characterController != null)
        {
            HandleMovement();  // �̵� ó��
            HandleRotation();  // ȸ�� ó��
        }
    }

    private void HandleMovement()
    {
        // L ��Ʈ�ѷ� (Primary Thumbstick)�θ� �̵�
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

        // ȸ�� �Է��� �����ϰ� �̵� ���͸� ���
        if (primaryAxis.magnitude > 0.1f)  // ���� ������ ���͸�
        {
            Vector3 moveDirection = new Vector3(primaryAxis.x, 0, primaryAxis.y);
            moveDirection = transform.TransformDirection(moveDirection) * ovrPlayerController.Acceleration;

            if (characterController.isGrounded && OVRInput.GetDown(OVRInput.Button.One))  // ����
            {
                moveDirection.y = ovrPlayerController.JumpForce;
            }

            // �߷� ����
            moveDirection.y += Physics.gravity.y * ovrPlayerController.GravityModifier * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);
        }
    }

    float x;
    float rotSpeed = 200.0f;
    private void HandleRotation()
    {
        // R ��Ʈ�ѷ� (Secondary Thumbstick)�θ� ȸ��
        Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        x += secondaryAxis.x * rotSpeed * Time.deltaTime;
        transform.eulerAngles = new Vector3(0, x, 0);
        //if (Mathf.Abs(secondaryAxis.x) > 0.1f)  // ���� ������ ����
        //{
        //    float rotationAmount = secondaryAxis.x * ovrPlayerController.RotationAmount;

        //    // �ε巯�� ȸ�� ����
        //    Quaternion targetRotation = Quaternion.Euler(0, rotationAmount, 0) * transform.rotation;
        //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        //}
    }
}
