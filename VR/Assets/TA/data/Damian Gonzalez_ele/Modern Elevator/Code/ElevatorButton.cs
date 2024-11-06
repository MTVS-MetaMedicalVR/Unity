using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModernElevator {
    public enum Direction { Up, Down }
    public class ElevatorButton : MonoBehaviour {
        public enum ButtonType { GoTo, LightToggle, Alarm, Stop }
        public ButtonType buttonType;
        public Transform stop;
        public Direction direction = Direction.Up;
        public ElevatorBrain brain;

        [Space(10)]
        [Header("Only for mechanical buttons ________________________________________")]
        [Space(40)]

        [SerializeField] private bool animateOnPress = true;
        [SerializeField] private AnimationCurve pressAnimationCurve;
        [SerializeField] private Vector3 movementFactor = new Vector3(-.2f, 0, 0);
        public MeshRenderer lightOutline;
        public bool isCallerButton = false;
        private float pressStartTime = 0;
        private float curveSpeed = 1f;
        private Vector3 originalLocalPosition;
        private bool isAnimating = false;


        public void Press(bool withSound = true) {
            //cool down time
            if (buttonType != ButtonType.LightToggle && Time.time < pressStartTime + 1f) return;

            pressStartTime = Time.time;
            bool randomizePitchAndVol=true;

            //main action
            switch (buttonType) {
                case ButtonType.GoTo:
                    brain.AddFloorToList(stop);
                    break;

                case ButtonType.Stop:
                    brain.EmergencyStopBegin();
                    break;

                    
                case ButtonType.LightToggle:
                    brain.ElevatorLightsToggle();
                    break;

                case ButtonType.Alarm:
                    SetLight(true);
                    Invoke(nameof(SetLightOff), 1f);
                    randomizePitchAndVol = false;
                    //sound plays automatically
                    break;
                    
            }

            if (withSound) {
                //if there is an audio source attached, play it
                if (TryGetComponent(out AudioSource audio)) {
                    if (randomizePitchAndVol) {
                        audio.pitch = Random.Range(.95f, 1.1f);
                        audio.volume = Random.Range(.8f, 1.2f);
                    }
                    audio.Play();
                }
            }

            if (animateOnPress) {
                isAnimating = true;
                curveSpeed = Random.Range(.8f, 1.2f);
            }

        }



        public void Start() {
            originalLocalPosition = transform.localPosition;
        }


        void FixedUpdate() {
            if (animateOnPress && isAnimating) {
                float elapsedTime = (Time.time - pressStartTime) * curveSpeed;
                if (elapsedTime > 3f) isAnimating = false;

                transform.localPosition =
                    originalLocalPosition
                    + (movementFactor * pressAnimationCurve.Evaluate(elapsedTime))
                ;
            }
        }

        public void SetLight(bool value) {
            lightOutline.material = value ? brain.buttonLightOn : brain.buttonLightOff;
        }

        private void SetLightOff() {
            SetLight(false);
        }

    }
}