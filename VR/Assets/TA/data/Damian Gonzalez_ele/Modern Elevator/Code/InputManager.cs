using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModernElevator {
    public class InputManager : MonoBehaviour {
        public static float vertical;
        public static float horizontal;
        public static float verticalMouse;
        public static float horizontalMouse;
        public static bool pressedJump = false;
        public static bool pressedInteract = false;
        public static bool pressingRun = false;


        public enum InputType { NewInputSystem, OldInputSystem }
        public InputType inputType;
        public float mouseSensitivity = 1f;


        void Update() {
            if (inputType == InputType.OldInputSystem) {
                vertical = Input.GetAxis("Vertical");
                horizontal = Input.GetAxis("Horizontal");
                verticalMouse = Input.GetAxis("Mouse Y") * mouseSensitivity;
                horizontalMouse = Input.GetAxis("Mouse X") * mouseSensitivity;
                pressedJump = Input.GetButtonDown("Jump");
                pressedInteract = Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.E);
                pressingRun = Input.GetKey(KeyCode.LeftShift);
            }
        }
    }
}