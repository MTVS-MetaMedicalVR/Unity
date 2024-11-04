using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModernElevator {
    public class DoorSwitch : MonoBehaviour {


        public ElevatorBrain brain;
        public Transform floor;
        [SerializeField] private Material matLightOff, matLightOn;

        MeshRenderer switchLights_mr;
        GameObject aditionalRedLight;
        AudioSource sound;
        Animator anim;
        int originalLayer;
        bool setupComplete = false;

        public void Setup() {
            switchLights_mr = transform.Find("SwitchLights")?.GetComponent<MeshRenderer>();
            aditionalRedLight = transform.Find("SwitchLights")?.GetChild(0)?.gameObject;
            sound = transform.Find("forcing door sound")?.GetComponent<AudioSource>();
            anim = GetComponent<Animator>();
            setupComplete = true;
        }

        public void Activate() {
            if (!setupComplete) Setup();

            //animate the switch itself
            anim.SetTrigger("Activate");

            //and remove the interactable layer
            originalLayer = gameObject.layer;
            gameObject.layer = 0;

            //sound
            sound?.Play();
            sound.time = .5f; //start later

            Invoke(nameof(ForceOpen), .5f);

        }

        public void ForceOpen() { 
            //start door animation
            foreach (AutomaticDoor door in brain.outerDoors) {
                if (door.floorStop == floor) {
                    //door.forced = true;
                    door.Open();
                }
            }

            //change light material
            switchLights_mr.material = matLightOff;

            //turn on aditional red light
            aditionalRedLight?.SetActive(true);

            //close in 10 seconds
            Invoke(nameof(DeactivateSwitch), 10f);
        }

        void DeactivateSwitch() {
            //start door animation
            foreach (AutomaticDoor door in brain.outerDoors) {
                if (door.floorStop == floor) {
                    //door.forced = true;
                    door.Close();
                }
            }

            //change light material
            switchLights_mr.material = matLightOn;

            //turn off aditional red light
            aditionalRedLight?.SetActive(false);

            //animate the switch itself
            //anim.SetTrigger("Deactivate");

            //and restore the interactable layer
            gameObject.layer = originalLayer;

            sound?.Play();
            sound.time = .5f; //start later
        }
    }
}