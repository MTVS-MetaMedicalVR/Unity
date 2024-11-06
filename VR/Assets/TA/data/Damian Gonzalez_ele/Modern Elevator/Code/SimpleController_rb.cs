using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ModernElevator {

    [RequireComponent(typeof(Rigidbody))]
    public class SimpleController_rb : MonoBehaviour {
        public static Transform thePlayer;        //easy access
        public static SimpleController_rb script; //easy access

        Rigidbody rb;
        Transform cam;
        float rotX = 0;
        public float distanceFloorToPlayer = 1f;
        public float walkSpeed = 2f;
        public float runSpeed = 4f;
        public float climbSpeed = 2f;
        public LayerMask layerMaskNotPlayer;


        //gravity:
        bool grounded;
        public float jumpForce = 4f;

        //climbing:
        int stairsAreas = 0;
        bool climbingHatchPart1 = false, climbingHatchPart2 = false;
        Transform positionForAnimation;

        public bool insideStairsTrigger = false;
        float climbingMovement;
        float walkingMovement;


        void Awake() {
            //easy access from anywhere:
            thePlayer = transform;
            script = this;


            rb = GetComponent<Rigidbody>();
            cam = transform.GetChild(0);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
            rotX = cam.eulerAngles.x;
        }


        void Update() {

            ManageHatchAnimation();
            if (climbingHatchPart1 || climbingHatchPart2) return;

            ManageGravityAndJumping();
            ManageMovement();
            ManageRotation();

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
                
            }
            if (climbingHatchPart2) {
                rotX = cam.eulerAngles.x; //absorb new camera rotation
            }
        }

        void ManageGravityAndJumping() {
            grounded = insideStairsTrigger || Physics.CheckSphere(transform.position - Vector3.up * distanceFloorToPlayer, .1f, layerMaskNotPlayer) ;

            rb.useGravity = !insideStairsTrigger;

            if (grounded) {
                //since he's grounded (or in stairs), he can jump
                if (InputManager.pressedJump) {
                    //jump
                    rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);

                    //if he is climbing, simulate an exit from the trigger
                    insideStairsTrigger = false;
                    stairsAreas = 0;
                }
            }
        }

        private void ManageMovement() {
            float speed = InputManager.pressingRun ? runSpeed : walkSpeed;
            Vector3 forwardNotTilted = new Vector3(transform.forward.x, 0, transform.forward.z);

            climbingMovement = 0;
            walkingMovement = 0;
            if (insideStairsTrigger) {
                climbingMovement = InputManager.vertical;

                //also go forward when climbing, so the player can touch the wall
                //walkingMovement = Mathf.Clamp01(climbingMovement);
            } else {
                walkingMovement = InputManager.vertical;
            }


            rb.velocity = (
                forwardNotTilted * speed * walkingMovement              //move forward
                +
                transform.right * speed * InputManager.horizontal       //slide to sides
                +
                Vector3.up * 
                    (insideStairsTrigger 
                        ? (climbingMovement * climbSpeed)       //climb
                        : rb.velocity.y                         //jump and fall
                    )                              

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

            rb.isKinematic = true;
            rb.useGravity = false;

            //part 1: by code, moves to the first animation position
            //part 2: only moves by animation
        }


        public void FinishedClimbing() {
            climbingHatchPart1 = false; //resume player control
            climbingHatchPart2 = false;

            GetComponent<Animator>().enabled = false;
            rotX = cam.eulerAngles.x; //absorb new camera rotation

            ElevatorInteraction.instance.RestoreObjectsAfterClimbing();
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }

}
