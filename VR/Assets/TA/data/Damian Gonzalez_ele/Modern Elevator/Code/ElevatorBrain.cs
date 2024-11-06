using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ModernElevator {
    [HelpURL("https://www.pipasjourney.com/damianGonzalez/modern_elevator/")]
    [DefaultExecutionOrder(1100)] //after DoorTrigger
    public class ElevatorBrain : MonoBehaviour {

        
        public static ElevatorBrain[] elevators;        //your elevators, always accesible (see documentation, section 9)
        [HideInInspector] public DoorSwitch[] switches; //the switches, always accesible (see documentation, section 9)


        [Space(40)]
        [Header("Speed ____________________________________________________________")]
        [Space(20)]
        [SerializeField] private float unitsPerSecond = 1.5f;
        [SerializeField] private bool slowDownNearArriving = true;
        [Range(.01f, .05f)] [SerializeField] private float slowingDownEffect = .03f;
        public Transform initialStop;
        
        [HideInInspector] public List<AutomaticDoor> outerDoors = new List<AutomaticDoor>();
        [HideInInspector] public List<AutomaticDoor> innerDoors = new List<AutomaticDoor>();



        [Space(40)]
        [Header("Avoid player bouncing while traveling _________________________")]
        [Space(20)]
        public bool avoidBouncing = true;
        public Transform player;
        [HideInInspector] public bool playerInsideElevator = false;


        [Space(40)]
        [Header("Not everything is pink ______________________________________________")]
        [Space(20)]
        [SerializeField] private FlickeringFrecuency lightFlickering = FlickeringFrecuency.AlmostUnnoticeable;
        public enum FlickeringFrecuency { NoFlickering, AlmostUnnoticeable, Annoying, Heavy, Nightmare }

        [SerializeField] private ElevatorShaking elevatorShaking = ElevatorShaking.AlmostUnnoticeable;
        public enum ElevatorShaking { NoShaking, AlmostUnnoticeable, Annoying, Heavy, Nightmare }

        [Range(0, 2f)] [SerializeField] private float shakeWhenTravelBegins = 1f;
        [Range(0, 2f)] [SerializeField] private float shakeWhenTravelEnds = .5f;
        //[Range(0, 2f)] [SerializeField] private float shakeOnEmergencyStop = 1.2f;
        [SerializeField] private float shakeLowThreshold = .01f;

        [Space(40)]
        [Header("Details ____________________________________________________________")]
        [Space(20)]

        [Range(0, 1.5f)] [SerializeField] private float delayStartMoving = 1f;
        [Range(0, 1.5f)] [SerializeField] private float delayOpeningDoors = 1f;
        [Range(2f, 6f)]  [SerializeField] private float waitTimeBetweenStops = 4f;
        [SerializeField] private bool debugMode = false;


        [Space(40)]
        [Header("Internal references ________________________________________________")]
        [Space(20)]
        public Transform elevator;
        public GameObject elevatorLightsContainer;
        public ElevatorButton elevatorLightSwitchScript;
        [HideInInspector] public Transform[] wireEndBone = new Transform[4];
        [HideInInspector] public Vector3[] wiresEndPosition = new Vector3[4];


        [Space(40)]
        [Header("Lights Materials ________________________________________")]
        [Space(20)]
        public Material buttonLightOn;
        public Material buttonLightOff;
        /*
        [SerializeField] private Material movingDownLight;
        [SerializeField] private Color movingLightColor = Color.green;
        [SerializeField] private Material emergencyStopLight;
        [SerializeField] private Color emergencyStopColor = Color.red;
        [SerializeField] private float callerLightsIntensity = 20f;
        */




        [Space(40)]
        [Header("Sounds _________________________________________________________")]
        [Space(20)]
        public AudioSource travelSound;
        public AudioSource stopSound;

        //public AudioSource emergencyStopSound;
        public AudioSource arrived_ding;
        public AudioSource arrived_Arp3NotesLow;
        public AudioSource arrived_Arp3NotesHigh;
        public AudioSource arrived_Arp2NotesHigh;

        public AudioSource doorOpensSound;
        public AudioSource doorClosesSound;

        //public AudioSource breaksSound;
        public AudioSource[] flickeringSounds = new AudioSource[3];

        public enum ArrivedSoundType { None, Ding, Arp3NotesLow, Arp3NotesHigh, Arp2NotesHigh }
        public ArrivedSoundType arrivedSound = ArrivedSoundType.Arp3NotesHigh;



        //non-exposed variables
        private List<Transform> listOfAllStops = new List<Transform>();
        private List<Transform> listOfPendingStops = new List<Transform>();
        private bool moveWhenDoorCloses = false;
        private Transform destination;

        [HideInInspector] public  Direction direction;
        private int currentFloor;
        [HideInInspector] public  bool idle = true;
        [HideInInspector] public  bool moving = false;
        [HideInInspector] public  int nearestFloor;

        private float nextFlicker = 1f;
        private float nextShaking = 1f;
        private float currentShakeMagnitude = 0;
        private float multiplier = 2f;
        private Quaternion originalElevatorRotation;
        private float currentSpeed;
        private bool breaking = false;
        private Transform stopsContainer;
        private bool lightsState = true;

        [HideInInspector] public List<Sprite> listOfNumberSprites = new List<Sprite>();

        private List<ElevatorButton> listOfElevatorButtons = new List<ElevatorButton>();
        private List<ElevatorButton> listOfCallerButtons = new List<ElevatorButton>();

        void Awake() {
            //do the hard work only once:

            //a) find and save doors on this elevator system
            innerDoors.Clear();
            outerDoors.Clear();

            foreach (AutomaticDoor door in transform.parent.GetComponentsInChildren<AutomaticDoor>()) {
                if (door.doorLocation == AutomaticDoor.DoorLocation.External) outerDoors.Add(door);
                if (door.doorLocation == AutomaticDoor.DoorLocation.Internal) innerDoors.Add(door);
            }

            
            //b) find and save every elevator stop in a list
            stopsContainer = transform.GetChild(0); //only child of this Brain
            listOfAllStops.Clear();
            for (int i = 0; i < stopsContainer.childCount; i++) {
                listOfAllStops.Add(stopsContainer.GetChild(i));
            }


            //c) find and save every button:
            listOfCallerButtons.Clear();
            listOfElevatorButtons.Clear();
            foreach (ElevatorButton btn in transform.parent.GetComponentsInChildren<ElevatorButton>()) {
                if (btn.isCallerButton)
                    listOfCallerButtons.Add(btn);
                else
                    listOfElevatorButtons.Add(btn);
            }


        }

        void Start() {
            originalElevatorRotation = elevator.rotation;
            elevator.Translate(0, initialStop.position.y - elevator.position.y, 0);
            destination = NearestFloor();
            moving = false;
            idle = true;
            //UpdateDoorLocks(true);
            UpdateButtonLights();
            UpdateDispays();
            UpdateWires();

            //player not provided?
            if (avoidBouncing && player == null) {
                //try to get the player by tag
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
            }

            InvokeRepeating(nameof(SlowUpdate), .2f, .2f);
            Invoke(nameof(OpenDoors), .1f);
        }

        public void AddFloorToList(int floor) {     // <- main verb (give it an int or transform. See documentation)
            AddFloorToList(stopsContainer.GetChild(floor));
        }

        public void AddFloorToList(Transform tr) {  // <- main verb (give it an int or transform. See documentation)
            if (listOfPendingStops.Contains(tr)) {
                if (debugMode) Debug.Log($"Floor {tr.name} is already in queue. Ignoring.");
                return;
            }
            if (ElevatorIsInStop(tr.position)) {
                if (debugMode) Debug.Log($"Elevator is already in floor {tr.name}. Ignoring.");
                return;
            }

            listOfPendingStops.Add(tr);
            if (debugMode) Debug.Log($"Added floor {tr.name} to queue", tr);
            if (idle) GoToNextStop();

            ReevaluateNextStop();
            UpdateButtonLights();
        }

        public void PurgeFloorList() {
            listOfPendingStops.Clear();
        }

        void SlowUpdate() { //once every 0.2 seconds. For things don't need to be in sync all the time
            UpdateDispays();
            UpdateButtonLights();
        }


        

        void GoToNextStop() {
            if (debugMode) Debug.Log("GoToNextStop");

            //this works with the "listOfPendingStops". 
            //If there is no stop in queue, just exit. Nothing to do until a button is pressed. The elevator enters idle mode.
            if (listOfPendingStops.Count == 0) {
                idle = true;
                if (debugMode) Debug.Log("Idle but no stops in queue. Elevator is now in idle mode.");
                return;
            }


            if (!idle) {
                Transform nextStop = FindNextStop(direction);

                if (nextStop != null) {
                    destination = nextStop;
                    if (debugMode) Debug.Log("New destination. Going " + direction.ToString() + " to " + nextStop.name, nextStop);
                } else { 
                    //there is no pending stop in the current direction, but there are pending stops
                    //enter idle mode, so next frame it goes either direction
                    idle = true;
                    GoToNextStop();
                    return;
                }
            } else {
                //if idle, any direction is good. Just go to the nearest pending stop (excluding the current one)
                Transform nearestPendingStop = null;
                
                float minDistance = 1000f;
                foreach (Transform tr in listOfPendingStops) {
                    float distance = Mathf.Abs(tr.position.y - elevator.position.y);
                    if (distance < minDistance) {
                        nearestPendingStop = tr;
                        minDistance = distance;
                    }
                }
                if (nearestPendingStop != null) {
                    destination = nearestPendingStop;
                    if (debugMode) Debug.Log("Idle. Going to nearest pending stop: " + nearestPendingStop.name, nearestPendingStop);
                    
                    direction = (Mathf.Sign(destination.position.y - elevator.position.y) == 1)
                                ? Direction.Up
                                : Direction.Down;
                } else {
                    if (debugMode) Debug.Log("Idle. There is no nearest pending stop.");
                }

            }

            UpdateButtonLights();
            UpdateDispays();


 
            currentSpeed = unitsPerSecond / 50f;
            moving = false; //not until door are fully closed
            idle = false;

            if (!innerDoors[0].isFullyClosed || !innerDoors[0].isFullyClosed) {
                CloseDoors();
            }
            moveWhenDoorCloses = true;



        }

        Transform FindNextStop(Direction direction) {
            float safeMargin = .5f;

            //if going up, look for next called floor above
            if (direction == Direction.Up) {
                for (int i = 0; i < stopsContainer.childCount; i++) {
                    Transform thisStop = stopsContainer.GetChild(i);
                    if (thisStop.position.y > elevator.position.y + safeMargin) {
                        if (listOfPendingStops.Contains(thisStop)) {
                            return thisStop;
                        }
                    }
                }
            }

            //if going down, look for next called floor below
            if (direction == Direction.Down) {
                for (int i = listOfAllStops.Count - 1; i >= 0; i--) {
                    Transform thisStop = listOfAllStops[i];
                    if (thisStop.position.y < elevator.position.y - safeMargin) {
                        if (listOfPendingStops.Contains(thisStop)) {
                            return thisStop;
                        }
                    }
                }
            }
            return null;
        }

        bool ElevatorIsInStop(Vector3 pos) {
            float safeMargin = .2f;
            return Mathf.Abs(pos.y - elevator.position.y) < safeMargin;
        }

        bool ElevatorIsInStop(Transform stop) {
            return ElevatorIsInStop(stop.position);
        }



        void UpdateButtonLights() {
            //buttons inside elevator
            foreach (ElevatorButton btn in listOfElevatorButtons) {
                if (btn.buttonType == ElevatorButton.ButtonType.GoTo) {
                    btn.SetLight(
                        listOfPendingStops.Contains(btn.stop) ||
                        ((moving || moveWhenDoorCloses) && destination == btn.stop)
                    );
                }
                if (btn.buttonType == ElevatorButton.ButtonType.LightToggle) {
                    btn.SetLight(lightsState);
                }
            }

            //caller buttons
            foreach (ElevatorButton btn in listOfCallerButtons) {
                btn.SetLight(
                    (listOfPendingStops.Contains(btn.stop) || 
                    ((moving || moveWhenDoorCloses) && destination == btn.stop))
                    && btn.direction == direction
                );
            }
        }

        
        void UpdateDispays() {
            nearestFloor = int.Parse(NearestFloor().name);

        }


        
        Transform NearestFloor() {
            Transform nearestStop = null;
            float minDistance = 1000f;
            foreach (Transform tr in listOfAllStops) {
                float distance = Mathf.Abs(tr.position.y - elevator.position.y);
                if (distance < minDistance) {
                    nearestStop = tr;
                    minDistance = distance;
                }
            }
            return nearestStop;

        }



        void StartMoving() {
            moving = true;
            currentShakeMagnitude = shakeWhenTravelBegins;
            travelSound?.Play();
        }        
        void FixedUpdate() {
            if (!moving && moveWhenDoorCloses && innerDoors[0].isFullyClosed && innerDoors[1].isFullyClosed) {
                //travel begins ... in a moment
                Invoke(nameof(StartMoving), delayStartMoving);
                moveWhenDoorCloses = false;

            }

            if (moving) {
                //linear movement

                elevator.Translate(0, ((direction == Direction.Up) ? 1f : -1f) * currentSpeed, 0);


                //almost there?
                if (slowDownNearArriving) {
                    if (
                       (direction == Direction.Up && elevator.position.y > destination.position.y - 1f)
                       ||
                       (direction == Direction.Down && elevator.position.y < destination.position.y + 1f)
                    ) {
                        currentSpeed = Mathf.Clamp(currentSpeed * (1 - slowingDownEffect), .002f, 99f);
                        if (!breaking) {
                            breaking = true;

                            if (travelSound != null) travelSound.Stop();
                            if (stopSound != null) stopSound.Play();

                        }
                    }
                }


                //has arrived?
                if (
                    (direction == Direction.Up && elevator.position.y > destination.position.y)
                    ||
                    (direction == Direction.Down && elevator.position.y < destination.position.y)
                ) {
                    currentShakeMagnitude = shakeWhenTravelEnds;

                    listOfPendingStops.Remove(destination);
                    moving = false;
                    breaking = false;
                    moveWhenDoorCloses = false;
                    //UpdateDoorLocks();
                    UpdateButtonLights();

                    Invoke(nameof(OpenDoors), delayOpeningDoors);

                    //play "arrived" sound
                    switch (arrivedSound) {
                        case ArrivedSoundType.Arp2NotesHigh: 
                            arrived_Arp2NotesHigh.Play();
                            break;

                        case ArrivedSoundType.Arp3NotesHigh:
                            arrived_Arp3NotesHigh.Play();
                            break;

                        case ArrivedSoundType.Arp3NotesLow:
                            arrived_Arp3NotesLow.Play();
                            break;

                        case ArrivedSoundType.Ding:
                            arrived_ding.Play();
                            break;


                    }

                    Invoke(nameof(GoToNextStop), delayOpeningDoors + waitTimeBetweenStops);
                }
            }

            if (currentShakeMagnitude > .01f) currentShakeMagnitude *= .9f;
        }



        [ContextMenu("Open doors")]
        public void OpenDoors() {

            //open inner doors
            foreach (AutomaticDoor door in innerDoors) {
                door.Open();
            }

            //open outter doors on this floor
            foreach (AutomaticDoor door in outerDoors) {
                if (Mathf.Abs(door.floorStop.position.y - elevator.position.y) < .1f) door.Open();
            }

            //sound
            doorOpensSound.Play();
        }

        void CloseDoors() {
            //close inner doors
            foreach (AutomaticDoor door in innerDoors) {
                door.Close();
            }

            //close outter doors on this floor
            foreach (AutomaticDoor door in outerDoors) {
                if ((door.floorStop.position.y - elevator.position.y) < .1f) door.Close();
            }

            doorClosesSound.Play();
            doorClosesSound.time = 1f;
        }


        bool ElevatorIsOnDestination() {
            return ElevatorIsInStop(destination);
        }


        void Update() {
            if (Time.time > nextFlicker) ApplyLightsFlickering();
            if (Time.time > nextShaking) ApplyElevatorShaking();

            if (elevatorShaking != ElevatorShaking.NoShaking && currentShakeMagnitude > shakeLowThreshold) {
                elevator.rotation = originalElevatorRotation * Quaternion.Euler(
                    Random.Range(-currentShakeMagnitude, currentShakeMagnitude),
                    Random.Range(-currentShakeMagnitude, currentShakeMagnitude),
                    Random.Range(-currentShakeMagnitude, currentShakeMagnitude)
                );

            }

            if (moving) UpdateWires();
        }

        void ApplyLightsFlickering() {
            
            if (!lightsState) return; //only flicker if light is (in theory) on
            float offDuration = 0, onDuration = 0;
            switch (lightFlickering) {
                case FlickeringFrecuency.NoFlickering:
                    nextShaking = float.MaxValue;
                    return;

                case FlickeringFrecuency.AlmostUnnoticeable:
                    offDuration = Random.Range(.05f, .1f);
                    onDuration = Random.Range(3f, 6f);
                    break;

                case FlickeringFrecuency.Annoying:
                    offDuration = Random.Range(.08f, .2f);
                    onDuration = Random.Range(.5f, 4f);
                    break;

                case FlickeringFrecuency.Heavy:
                    offDuration = Random.Range(.08f, .2f);
                    onDuration = Random.Range(.2f, 2f);
                    break;

                case FlickeringFrecuency.Nightmare:
                    offDuration = Random.Range(.1f, 1.5f);
                    onDuration = Random.Range(.05f, .5f);
                    break;
            }
            elevatorLightsContainer.gameObject.SetActive(false);

            Invoke("LightFlickerEnds", offDuration);
            nextFlicker = Time.time + offDuration + onDuration;
            flickeringSounds[Random.Range(0, 3)].Play();
            
        }



        void ApplyElevatorShaking() {
            if (!moving) return;
            switch (elevatorShaking) {
                case ElevatorShaking.NoShaking:
                    nextShaking = float.MaxValue;
                    return;

                case ElevatorShaking.AlmostUnnoticeable:
                    currentShakeMagnitude = Random.Range(.05f, .1f) * multiplier;
                    nextShaking = Time.time + Random.Range(2f, 6f);
                    break;

                case ElevatorShaking.Annoying:
                    currentShakeMagnitude = Random.Range(.08f, .2f) * multiplier;
                    nextShaking = Time.time + Random.Range(.5f, 4f);
                    break;

                case ElevatorShaking.Heavy:
                    currentShakeMagnitude = Random.Range(.08f, .2f) * multiplier;
                    nextShaking = Time.time + Random.Range(.2f, 2f);
                    break;

                case ElevatorShaking.Nightmare:
                    currentShakeMagnitude = Random.Range(.1f, 1f) * multiplier;
                    nextShaking = Time.time + Random.Range(.5f, 1f);
                    break;
            }

        }

        void LightFlickerEnds() {
            if (lightsState) {
                elevatorLightsContainer.gameObject.SetActive(true);
            }
        }


        public void ElevatorLightsToggle() {
            lightsState = !lightsState;
            elevatorLightsContainer.gameObject.SetActive(lightsState);
            CancelInvoke(nameof(LightFlickerEnds));
            UpdateButtonLights();
            flickeringSounds[Random.Range(0, 3)].Play();
        }



        public void EmergencyStopBegin() {

        }

        public void EmergencyStopEnds() {

        }

        public void UpdateWires() { //stretch the wire to reach the top engine
            for (int i = 0; i < 4; i++) {
                wireEndBone[i].position = wiresEndPosition[i];
            }
        }
        

        public void PlayForcingSound() {
            doorClosesSound.Play();
        }

        public void MakePitNoisesLouder() {
            travelSound.volume = 1f;
            stopSound.volume = 1f;
        }

        void ReevaluateNextStop() {
            //maybe there is a nearest pending stop in the current direction. Go there.

            Transform nextStop = FindNextStop(direction);
            if (nextStop != null) {
                destination = nextStop;
                if (debugMode) Debug.Log("New destination. Going " + direction.ToString() + " to " + nextStop.name, nextStop);
            }
        }

    }
}