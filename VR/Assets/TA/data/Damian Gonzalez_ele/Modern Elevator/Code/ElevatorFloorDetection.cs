using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModernElevator {
    public class ElevatorFloorDetection : MonoBehaviour {
        public ElevatorBrain brain;
        Transform previousParent;

        void OnTriggerEnter(Collider col) {
            if (brain.avoidBouncing && col.transform == brain.player) {
                previousParent = brain.player.parent;
                brain.player.parent = brain.elevator; //brain.playerInsideElevator = true;
            }
        }

        void OnTriggerExit(Collider col) {
            if (brain.avoidBouncing && col.transform == brain.player) {
                brain.player.parent = previousParent;
            }
        }
    }
}