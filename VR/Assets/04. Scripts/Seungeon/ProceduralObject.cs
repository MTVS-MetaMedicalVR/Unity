using UnityEngine;
using System;

[Flags]
public enum InteractionType
{
    None = 0,
    LeftTrigger = 1 << 0,
    LeftGrip = 1 << 1,
    LeftThumbstick = 1 << 2,
    LeftPrimaryButton = 1 << 3,  // X button
    LeftSecondaryButton = 1 << 4, // Y button
    RightTrigger = 1 << 5,
    RightGrip = 1 << 6,
    RightThumbstick = 1 << 7,
    RightPrimaryButton = 1 << 8,  // A button
    RightSecondaryButton = 1 << 9, // B button
    OnTriggerEnter = 1 << 10,
    OnTriggerStay = 1 << 11,
    OnTriggerExit = 1 << 12
}

[System.Serializable]
public class ControllerState
{
    public bool isTriggerPressed;
    public bool isGripPressed;
    public bool isThumbstickPressed;
    public bool isPrimaryButtonPressed;
    public bool isSecondaryButtonPressed;
    public bool isColliding;
    public Vector2 thumbstickValue;
    public float triggerValue;
    public float gripValue;
}

public class ProceduralObject : MonoBehaviour
{
    [SerializeField]
    private InteractionType allowedInteractions;

    [Header("Threshold Values")]
    [SerializeField, Range(0f, 1f)]
    private float triggerThreshold = 0.1f;
    [SerializeField, Range(0f, 1f)]
    private float gripThreshold = 0.1f;
    [SerializeField, Range(0f, 1f)]
    private float thumbstickThreshold = 0.1f;

    // 각 컨트롤러의 상태를 저장
    public ControllerState leftController = new ControllerState();
    public ControllerState rightController = new ControllerState();

    private void Update()
    {
        UpdateLeftControllerState();
        UpdateRightControllerState();
    }

    private void UpdateLeftControllerState()
    {
        if (HasInteraction(InteractionType.LeftTrigger))
        {
            leftController.triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
            leftController.isTriggerPressed = leftController.triggerValue > triggerThreshold;
        }

        if (HasInteraction(InteractionType.LeftGrip))
        {
            leftController.gripValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
            leftController.isGripPressed = leftController.gripValue > gripThreshold;
        }

        if (HasInteraction(InteractionType.LeftThumbstick))
        {
            leftController.thumbstickValue = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            leftController.isThumbstickPressed = leftController.thumbstickValue.magnitude > thumbstickThreshold;
        }

        if (HasInteraction(InteractionType.LeftPrimaryButton))
        {
            leftController.isPrimaryButtonPressed = OVRInput.Get(OVRInput.Button.Three); // X button
        }

        if (HasInteraction(InteractionType.LeftSecondaryButton))
        {
            leftController.isSecondaryButtonPressed = OVRInput.Get(OVRInput.Button.Four); // Y button
        }
    }

    private void UpdateRightControllerState()
    {
        if (HasInteraction(InteractionType.RightTrigger))
        {
            rightController.triggerValue = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger);
            rightController.isTriggerPressed = rightController.triggerValue > triggerThreshold;
        }

        if (HasInteraction(InteractionType.RightGrip))
        {
            rightController.gripValue = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
            rightController.isGripPressed = rightController.gripValue > gripThreshold;
        }

        if (HasInteraction(InteractionType.RightThumbstick))
        {
            rightController.thumbstickValue = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            rightController.isThumbstickPressed = rightController.thumbstickValue.magnitude > thumbstickThreshold;
        }

        if (HasInteraction(InteractionType.RightPrimaryButton))
        {
            rightController.isPrimaryButtonPressed = OVRInput.Get(OVRInput.Button.One); // A button
        }

        if (HasInteraction(InteractionType.RightSecondaryButton))
        {
            rightController.isSecondaryButtonPressed = OVRInput.Get(OVRInput.Button.Two); // B button
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HasInteraction(InteractionType.OnTriggerEnter)) return;

        if (other.CompareTag("LeftController"))
        {
            leftController.isColliding = true;
        }
        else if (other.CompareTag("RightController"))
        {
            rightController.isColliding = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!HasInteraction(InteractionType.OnTriggerStay)) return;

        // OnTriggerEnter에서 이미 상태가 설정되어 있으므로 
        // 추가적인 처리가 필요한 경우에만 구현
    }

    private void OnTriggerExit(Collider other)
    {
        if (!HasInteraction(InteractionType.OnTriggerExit)) return;

        if (other.CompareTag("LeftController"))
        {
            leftController.isColliding = false;
        }
        else if (other.CompareTag("RightController"))
        {
            rightController.isColliding = false;
        }
    }

    private bool HasInteraction(InteractionType type)
    {
        return (allowedInteractions & type) == type;
    }

    // 외부에서 상태 확인을 위한 편의 메서드들
    public bool IsLeftControllerInteracting => leftController.isColliding;
    public bool IsRightControllerInteracting => rightController.isColliding;
    public bool IsAnyControllerInteracting => leftController.isColliding || rightController.isColliding;

    public bool CheckCombinedCondition(InteractionType[] conditions)
    {
        foreach (var condition in conditions)
        {
            if (!HasInteraction(condition)) continue;

            bool conditionMet = condition switch
            {
                InteractionType.LeftTrigger => leftController.isTriggerPressed,
                InteractionType.LeftGrip => leftController.isGripPressed,
                InteractionType.LeftThumbstick => leftController.isThumbstickPressed,
                InteractionType.LeftPrimaryButton => leftController.isPrimaryButtonPressed,
                InteractionType.LeftSecondaryButton => leftController.isSecondaryButtonPressed,
                InteractionType.RightTrigger => rightController.isTriggerPressed,
                InteractionType.RightGrip => rightController.isGripPressed,
                InteractionType.RightThumbstick => rightController.isThumbstickPressed,
                InteractionType.RightPrimaryButton => rightController.isPrimaryButtonPressed,
                InteractionType.RightSecondaryButton => rightController.isSecondaryButtonPressed,
                _ => false
            };

            if (!conditionMet) return false;
        }
        return true;
    }
}