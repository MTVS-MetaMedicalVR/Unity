using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModernElevator {
    public class DigitalDisplay : MonoBehaviour {
        public ElevatorBrain brain;
        private SpriteRenderer directionSprite;
        private SpriteRenderer firstDigitSprite;
        private SpriteRenderer secondDigitSprite;

        void Start() {
            directionSprite = transform.GetChild(0)?.GetComponent<SpriteRenderer>();
            firstDigitSprite = transform.GetChild(1)?.GetComponent<SpriteRenderer>();
            secondDigitSprite = transform.GetChild(2)?.GetComponent<SpriteRenderer>();
        }


        void Update() {
            if (brain != null) {
                if (brain.idle) {
                    directionSprite.sprite = null;
                } else {
                    directionSprite.sprite = brain.listOfNumberSprites[
                        (brain.direction == Direction.Up) ? 11 : 10
                    ];
                }


                secondDigitSprite.sprite = brain.listOfNumberSprites[brain.nearestFloor % 10];
                int firstDigit = int.Parse(brain.nearestFloor.ToString("00").Substring(0, 1));
                firstDigitSprite.sprite = (firstDigit == 0) ? null : brain.listOfNumberSprites[firstDigit];

                
            }
        }
    }
}