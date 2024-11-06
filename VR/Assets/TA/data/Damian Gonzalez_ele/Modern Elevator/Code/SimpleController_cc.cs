 using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ModernElevator {

    [RequireComponent(typeof (CharacterController))]
    public class SimpleController_cc : MonoBehaviour {
        public static Transform thePlayer;        //easy access
        public static SimpleController_cc script; //easy access

        CharacterController cc;
        Transform cam;
        float rotX = 0;
        public float walkSpeed = 3f;
        public float runSpeed = 5f;
        public float climbSpeed = 2f;


        //gravity:
        public bool grounded;
        public float gravity = 9.81f;
        float verticalVelocity = 0;
        public float jumpForce = 4f;

        //climbing:
        int stairsAreas = 0;
        bool climbingHatchPart1 = false, climbingHatchPart2 = false;
        public Transform positionForAnimation;

        public bool insideStairsTrigger = false;
        float climbingMovement;
        float walkingMovement;


        void Awake() {
            //easy access from anywhere:
            thePlayer = transform;
            script = this;


            cc = GetComponent<CharacterController>();
            cam = transform.GetChild(0);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            rotX = cam.eulerAngles.x;
        }


        void Update() {

            ManageHatchAnimation();
            if (climbingHatchPart1 || climbingHatchPart2) return;

            ManageGravity();
            ManageMovement();
            ManageRotation();

            if (Input.GetKeyDown(KeyCode.KeypadPlus)) transform.GetChild(0).position += Vector3.up * .2f;
            if (Input.GetKeyDown(KeyCode.KeypadMinus)) transform.GetChild(0).position -= Vector3.up * .2f;
            if (Input.GetKeyDown(KeyCode.Alpha1)) UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) UnityEngine.SceneManagement.SceneManager.LoadScene(1);


        }
        void ManageHatchAnimation() {
            if (climbingHatchPart1) {
                transform.position = Vector3.Lerp(
                    transform.position,
                    positionForAnimation.position,
                    Time.deltaTime * 3f
                );
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    positionForAnimation.rotation,
                    Time.deltaTime * 3f
                );
                cam.localRotation = Quaternion.Lerp(
                    cam.localRotation,
                    Quaternion.Euler(-40, 0, 0),
                    Time.deltaTime * 3f
                );
                if (Vector3.Distance(transform.position, positionForAnimation.position) < .01f) {
                    //ready for animation
                    climbingHatchPart1 = false;
                    climbingHatchPart2 = true;

                    Animator anim = GetComponent<Animator>();
                    anim.enabled = true;
                    anim.SetTrigger("ClimbHatch");
                }
                return;
            }
            if (climbingHatchPart2) {
                rotX = cam.eulerAngles.x; //absorb new camera rotation
                return;
            }
        }

        void ManageGravity() {
            grounded = cc.isGrounded || insideStairsTrigger;

            //if not grounded, make the player fall
            if (!grounded) {
                verticalVelocity -= gravity * Time.deltaTime;
            } else {
                verticalVelocity = -.1f;

                //since he's grounded, he can jump
                if (InputManager.pressedJump) {
                    //jump
                    verticalVelocity = jumpForce;

                    //if he is climbing, simulate an exit from the trigger
                    insideStairsTrigger = false;
                    stairsAreas = 0;
                }
            }
        }
       
        private void ManageMovement() {
            if (climbingHatchPart1 || climbingHatchPart2) return;
            float speed = InputManager.pressingRun ? runSpeed : walkSpeed;
            Vector3 forwardNotTilted = new Vector3(transform.forward.x, 0, transform.forward.z);

            climbingMovement = 0;
            walkingMovement = 0;
            if (insideStairsTrigger) {
                climbingMovement = InputManager.vertical;

                //no gravity when climbing
                if (verticalVelocity < 0f) verticalVelocity = 0f;

                //also go forward when climbing, so the player can touch the wall
                walkingMovement = Mathf.Clamp01(climbingMovement);
            } else {
                walkingMovement = InputManager.vertical;
            }


            cc.Move(
                forwardNotTilted * speed * walkingMovement * Time.deltaTime             //move forward
                +
                transform.right * speed * InputManager.horizontal * Time.deltaTime      //slide to sides
                +
                Vector3.up * verticalVelocity * Time.deltaTime                          //jump and fall
                +
                Vector3.up * climbingMovement * climbSpeed * Time.deltaTime             //climb
            );


        }

        private void ManageRotation() {
            //rotate player left/right, and try to make player stand straight if tilted
            transform.rotation = (Quaternion.Lerp(
                transform.rotation * Quaternion.Euler(0, InputManager.horizontalMouse, 0),
                Quaternion.Euler(0, transform.eulerAngles.y, 0),
                .1f
            ));

            //look up and down
            rotX += InputManager.verticalMouse * -1;
            rotX = Mathf.Clamp(rotX, -90f, 90f); //clamp look 
            cam.localRotation = Quaternion.Euler(rotX, 0, 0);
        }



        void OnCollisionEnter(Collision col) {
            Debug.Log(col.gameObject.name);
        }

        
        void OnTriggerEnter(Collider col) {
            if (col.name.Contains("Stair")) {
                insideStairsTrigger = true;
                stairsAreas++;
            }
        }

        void OnTriggerExit(Collider col) {
            if (col.name.Contains("Stair")) {
                stairsAreas--;
                if (stairsAreas < 0) stairsAreas = 0;
                if (stairsAreas == 0) insideStairsTrigger = false;
            }
        }


        public void Climb(Transform _positionForAnimation) {
            positionForAnimation = _positionForAnimation; //ref to the moving object
            climbingHatchPart1 = true; //can't move manually
            climbingHatchPart2 = false;

            //part 1: by code, moves to the first animation position
            //part 2: only moves by animation
        }


        public void FinishedClimbing() {
            climbingHatchPart1 = false; //resume player control
            climbingHatchPart2 = false;

            GetComponent<Animator>().enabled = false;
            rotX = cam.eulerAngles.x; //absorb new camera rotation

            ElevatorInteraction.instance.RestoreObjectsAfterClimbing();
            
        }
    }

}
