using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModernElevator {
    public class AutomaticDoor : MonoBehaviour {
        public ElevatorBrain brain;
        public bool isFullyClosed = true;
        public bool isFullyOpen = false;

        public enum DoorLocation { Internal, External }
        public DoorLocation doorLocation = DoorLocation.External;

        float closedPositionX;
        float openPositionX;
        float desiredPositionX;
        float desiredScaleX;
        public bool isRightPane = false;


        private bool opening = false;
        private bool closing = false;
        [Range(.02f, .1f)] public float howFast = .05f;
        public float randomSpeedFactor = .1f; //.1 means a 10% slower or faster than "howFast"
        [Range(.05f, .4f)] public float currentSpeed = .1f;

        public bool locked = true;
        public bool forced = false;

        public Transform floorStop;
        public float minDist = .01f;


        void Awake() {

            //get positions
            closedPositionX = transform.localPosition.x;
            openPositionX = closedPositionX + (.9f * (isRightPane ? -1f : 1f));
            desiredPositionX = closedPositionX;
            desiredScaleX = 1f;

        }


        [ContextMenu("Open")]
        public void Open() {
            desiredPositionX = openPositionX;
            desiredScaleX = .5f;

            currentSpeed = Random.Range(
                howFast / (1 + randomSpeedFactor),
                howFast * (1 + randomSpeedFactor)
            );

            opening = true;
            closing = false;
            isFullyOpen = false;
            isFullyClosed = false;


        }

        [ContextMenu("Close")]
        public void Close() {
            desiredPositionX = closedPositionX;
            desiredScaleX = 1f;

            currentSpeed = Random.Range(
                howFast / (1 + randomSpeedFactor),
                howFast * (1 + randomSpeedFactor)
            );

            opening = false;
            closing = true;
            isFullyOpen = false;
            isFullyClosed = false;

        }

        void FixedUpdate() {
            if (opening || closing) {
                transform.localPosition = new Vector3(
                    Mathf.Lerp(transform.localPosition.x, desiredPositionX, currentSpeed),
                    transform.localPosition.y,
                    transform.localPosition.z
                );

                transform.localScale = new Vector3(
                    Mathf.Lerp(transform.localScale.x, desiredScaleX, currentSpeed),
                    1f,
                    1f
                );
            }

            //opening stops?
            if (opening && Mathf.Abs(transform.localPosition.x - desiredPositionX) < minDist) {
                opening = false;
                isFullyOpen = true;
            }

            //closing stops?
            if (closing && Mathf.Abs(transform.localPosition.x - desiredPositionX) < minDist) {
                closing = false;
                isFullyClosed = true;
            }
        }

        public void ClosingEnds() { 
            isFullyClosed = true;
            closing = false;

            //if (doorLocation == DoorLocation.Internal) brain.InnerDoorsChangeState();
            //if (doorLocation == DoorLocation.External) brain.OuterDoorsChangeState();


        }

        private void OpeningEnds() {

            isFullyOpen = true;
            opening = false;

            //if (doorLocation == DoorLocation.Internal) brain.InnerDoorsChangeState();

        }


        public void Toggle() {
            if (isFullyClosed) Open();
            else Close();

        }


    }
}