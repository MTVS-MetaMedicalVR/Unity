using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModernElevator {
    public class ElevatorInteraction : MonoBehaviour {
        public static ElevatorInteraction instance;
        public LayerMask layerMaskButtons;
        public float maxInteractionDistance;
        public UnityEngine.UI.Image interactIcon;

        [SerializeField] List<GameObject> objectsToHideOnClimbing;
        [SerializeField] List<GameObject> objectsToShowOnClimbing;

        void Awake() {
            instance = this;
        }

        void Start() {
            if (layerMaskButtons == 0) layerMaskButtons = LayerMask.GetMask("ElevatorInteraction");
        }


        void Update() {
            if (Physics.Raycast(
                transform.position,
                transform.forward,
                out RaycastHit hit,
                maxInteractionDistance,
                ~0, //everything
                QueryTriggerInteraction.Collide
            )) {
                if (IsInLayerMask(hit.transform.gameObject.layer, layerMaskButtons)) {
                    //player can interact with an elevator interactable object
                    InteractionIcon.inst.SetInteractable(true); //-> this changes the circle icon


                    if (InputManager.pressedInteract) {//-> you can change this for: if (Input.GetKeyDown(KeyCode.E)) 


                        if (hit.collider.transform.TryGetComponent(out ElevatorButton btn)) {
                            //it is a button. Press it.
                            btn.Press();
                            return;
                        }

                        if (hit.collider.transform.TryGetComponent(out AutomaticDoor door)) {
                            //it is a door. Open/Close it.
                            door.Toggle();
                            return;
                        }

                        /*
                        if (hit.collider.transform.TryGetComponent(out RoomDoor rDoor)) {
                            //it is a room door. Open/Close it.
                            rDoor.Toggle();
                            return;
                        }
                        */

                        if (hit.collider.transform.name == "ElevatorHatchPanel") {
                            //elevator hatch panel
                            hit.collider.GetComponent<Animator>().SetTrigger("OpenHatchPanel");
                            hit.collider.gameObject.layer = 0; //no longer interactable

                            //make pit sounds louder
                            hit.collider.transform.parent.parent.Find("Elevator Brain")
                                .GetComponent<ElevatorBrain>().MakePitNoisesLouder();

                            return;
                        }

                        if (hit.collider.transform.name == "ElevatorHatchTrigger" //elevator hatch panel
                            && transform.position.y < hit.transform.position.y    //and from below
                        ) {
                            if (SimpleController_cc.script != null) {
                                SimpleController_cc.script.Climb(hit.transform.GetChild(0));

                            }

                            if (SimpleController_rb.script != null) {
                                SimpleController_rb.script.Climb(hit.transform.GetChild(0));
                            }


                            foreach (GameObject obj in objectsToHideOnClimbing) obj.SetActive(false);
                            foreach (GameObject obj in objectsToShowOnClimbing) obj.SetActive(true);

                        }

                        if (hit.collider.transform.name.Contains("Switch")) {
                            //it's a door switch on the pit, activate it
                            hit.collider.transform.GetComponent<DoorSwitch>().Activate();
                            
                        }

                    }
                } else {
                    //can't interact
                    InteractionIcon.inst.SetInteractable(false);
                }
            } else {
                //can't interact
                InteractionIcon.inst.SetInteractable(false);
            }
        }

        public void RestoreObjectsAfterClimbing() {
            foreach (GameObject obj in objectsToHideOnClimbing) obj.SetActive(true);
            foreach (GameObject obj in objectsToShowOnClimbing) obj.SetActive(false);
        }

        public bool IsInLayerMask(int layer, LayerMask layermask) {
            return layermask == (layermask | (1 << layer));
        }
    }
}