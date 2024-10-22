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
            HandleMovement();
            HandleRotation();
        }
    }

    private void HandleMovement()
    {
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector3 moveDirection = new Vector3(primaryAxis.x, 0, primaryAxis.y);
        moveDirection = transform.TransformDirection(moveDirection) * ovrPlayerController.Acceleration;

        if (characterController.isGrounded && OVRInput.GetDown(OVRInput.Button.One))
        {
            moveDirection.y = ovrPlayerController.JumpForce;
        }

        moveDirection.y += Physics.gravity.y * ovrPlayerController.GravityModifier * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        if (secondaryAxis.x != 0)
        {
            float rotationAmount = secondaryAxis.x * ovrPlayerController.RotationAmount;
            transform.Rotate(0, rotationAmount, 0);
        }
    }
}
