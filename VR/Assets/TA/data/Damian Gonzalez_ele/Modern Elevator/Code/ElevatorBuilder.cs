#if (UNITY_EDITOR)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ModernElevator {

    [HelpURL("https://www.pipasjourney.com/damianGonzalez/modern_elevator/")]
    public class ElevatorBuilder : MonoBehaviour {
        [Space(40)]
        [Header("▆  NEW ELEVATOR SETUP _________________________________________")]
        [Space(20)]
        [SerializeField] [Range(1, 5)] private int howManyElevators = 2;
        [SerializeField] [Range(2, 30)] private int howManyFloors = 5;
        [SerializeField] private bool addDemoHalls = true;
        [SerializeField] private bool addBackWallInHalls = true;
        [SerializeField] private float distanceBetweenFloors = 3f;
        [SerializeField] private float distanceBetweenLobbyAnd2ndFloor = 5f;
        [SerializeField] private Transform player;
        [SerializeField] private bool placePlayerInside;



        [Space(40)]
        [Header("▆  FLOOR NUMERATION _________________________________________")]
        [Space(20)]
        [SerializeField] private int firstFloorNumber = 1;
        [SerializeField] private bool forceTwoDigitsInFloorNumbers = false;
        [SerializeField] private bool addFloorNumbersInPitWalls = true;


        [Space(40)]
        [Header("▆  STYLE SETUP _______________________________________________")]
        [Space(20)]

        [SerializeField] private bool lobbyAsFirstFloor = true;

        public enum FrontType { None, LightMarble, DarkMarble }
        [SerializeField] private FrontType frontTypeLobby = FrontType.LightMarble;
        [SerializeField] private FrontType frontTypeOtherFloors = FrontType.DarkMarble;
        public enum NumbersType { None, Mirror, LightMarble, DarkMarble, Classic }
        [SerializeField] NumbersType numbersTypeInHallWalls = NumbersType.DarkMarble;
        [SerializeField] NumbersType numbersTypeInFront = NumbersType.Classic;

        public enum CallerSide { AlwaysLeft, AlwaysRight, MirroredInner, MirroredOuter }
        public CallerSide callerSide = CallerSide.MirroredInner;

        //[SerializeField] private bool mirrorOnElevatorBackWall = false; //-> not in build-in for now
        [SerializeField] private bool addDustParticlesInLobby = false;



        [Space(40)]
        [Header("▆  BUTTONS PAD SETUP _________________________________________")]
        [Space(20)]
        [SerializeField] private ButtonsLayout buttonsLayout = ButtonsLayout.Auto;
        enum ButtonsLayout { SingleColumn, TwoColumns, ThreeColumns, Auto }
        [SerializeField] private float verticalSpaceBetweenButtons = 0.5f;
        [SerializeField] private bool addAlarmButton = true;
        [SerializeField] private bool addLightsButton = true;


        [Space(40)]
        [Header("▆  LIGHTING _________________________________________________")]
        [Space(20)]
        [SerializeField] private LightsType lobbyLightsType = LightsType.Baked;
        [SerializeField] LightsAmount lobbyLightsAmount = LightsAmount.Reduced;

        [Space(10)]
        [SerializeField] private LightsType hallsLightsType = LightsType.Baked;
        //[SerializeField] LightsAmount hallsLightsAmount = LightsAmount.Reduced;

        [Space(10)]
        [SerializeField] private LightsType elevatorLightsType = LightsType.Realtime;
        [SerializeField] LightsAmount elevatorLightsAmount = LightsAmount.Reduced;

        [Space(10)]
        [SerializeField] private LightsType pitLightsType = LightsType.NoLights;
        [SerializeField] LightsAmount pitLightsAmount = LightsAmount.Reduced;

        [Space(10)]
        [SerializeField] private bool addReflectionProbeInLobby = false;
        [SerializeField] private bool addReflectionProbesInOthers = true;
        [SerializeField] private bool addShadowCasters = false;

        public enum LightsType { NoLights, Baked, Realtime, Mixed }
        public enum LightsAmount { Full, Reduced }



        [Space(40)]
        [Header("▆  ADVANCED ____________________________________________")]
        [Space(20)]

        [SerializeField] private PrefabsReferences refs;
        [System.Serializable]
        public class PrefabsReferences {
            public GameObject elevatorBrain;
            public GameObject mainElevator;
            public GameObject hallA;

            public GameObject entranceCase;
            public GameObject innerCase;
            public GameObject topCase;
            public GameObject bottomCase;
            public GameObject buttonAndNumber;
            public GameObject numberInWallMirror;
            public GameObject numberInWallMarble;
            public GameObject numberInWallMirrorHalf;
            public GameObject frontLightMarble;
            public GameObject frontDarkMarble;
            public GameObject hallBackWall;

            public Sprite[] fancyNumbersInWalls = new Sprite[10];
            public Sprite[] rustyNumbersInPit = new Sprite[10];
            public GameObject[] brailleNumbersPrefabs = new GameObject[10];
            public GameObject[] metallicNumbersPrefabs = new GameObject[10];
            //note: digital numbers for displays are stored in "ElevatorBrain", because it always uses them

            public GameObject lobbyLeftEnd;
            public GameObject lobbyMiddle;
            public GameObject lobbyRightEnd;

            public Material darkMarbleForPlaques;

            public GameObject dustParticles;
            public GameObject reflectionProbe;
        }
        public enum InstantiateMethod { PlacePrefabs, PlaceInstances }
        [SerializeField] InstantiateMethod instantiateMethod = InstantiateMethod.PlacePrefabs;

        //other internal variables
        private int floorNumber1stDigit;
        private int floorNumber2ndDigit;



        [ContextMenu("▶   Build")]
        public void Build() {


            //__________________________ PART 1: GENERAL SETUP __________________________

            //first of all, let's temporally straighten this builder
            Quaternion originalBuilderRotation = transform.rotation;
            transform.rotation = Quaternion.identity;


            //and initialize the static references for easy access
            ElevatorBrain.elevators = new ElevatorBrain[howManyElevators];

            
            //calculate center
            const float deltaX = -4f;
            float centerX = (howManyElevators - 1) * (deltaX / 2f);
            Vector3 centerOfThisFloor = transform.position + new Vector3(centerX, 0, 4f);


            //player not provided? try to get it by tag
            if (player == null) {
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
            }


            //main container
            Transform generalContainer = new GameObject("My modern elevator").transform;
            generalContainer.parent = transform;
            generalContainer.localPosition = Vector3.zero;
            generalContainer.localRotation = Quaternion.identity;


            //place player
            if (player != null && placePlayerInside) {
                player.position = generalContainer.position + new Vector3(centerX, -1f, lobbyAsFirstFloor ? 10f : 2f);
            }


            //lobby, except middle part(s)
            Transform lobby = null;
            if (lobbyAsFirstFloor) {
                lobby = new GameObject("Lobby").transform; 
                lobby.parent = generalContainer;
                lobby.localPosition = Vector3.zero;
                lobby.localRotation = Quaternion.identity;

                if (addDustParticlesInLobby) {
                    InstPrefab(
                        refs.dustParticles, 
                        new Vector3(howManyElevators / 2f, 0, 0), 
                        Quaternion.identity, 
                        lobby
                    );
                }

                Transform tempLobby;
                tempLobby = InstPrefab(refs.lobbyLeftEnd, 0, 0, lobby).transform;
                SetShadowCasters(tempLobby);

                tempLobby = InstPrefab(
                    refs.lobbyRightEnd,
                    new Vector3((howManyElevators - 1) * deltaX, 0, 0),
                    Quaternion.identity,
                    lobby
                ).transform;
                SetShadowCasters(tempLobby);

                //refl. probe in lobby
                if (addReflectionProbeInLobby) {
                    InstPrefab(refs.reflectionProbe, centerOfThisFloor, Quaternion.identity, lobby);
                }
            }


            //__________________________ PART 2: EACH ELEVATOR ____________________________
            for (int elevatorIndex = 0; elevatorIndex < howManyElevators; elevatorIndex++) {

                Transform thisContainer;
                if (howManyElevators == 1) {
                    thisContainer = generalContainer;
                }
                else {

                    thisContainer = new GameObject("Elevator nº " + (elevatorIndex + 1).ToString()).transform;
                    thisContainer.parent = generalContainer;
                    thisContainer.localPosition = new Vector3(deltaX * elevatorIndex, 0, 0);
                    thisContainer.localRotation = Quaternion.identity;
                    thisContainer.localScale = new Vector3(1, 1, 1);
                }



                //elevator itself
                float y = 0;
                GameObject elevator = InstPrefab(refs.mainElevator, new Vector3(0, y, -0.032f), Quaternion.identity, thisContainer);


                //elevator brain
                Transform brainTransform = InstPrefab(refs.elevatorBrain, y, 0, thisContainer).transform;


                ElevatorBrain brain = brainTransform.GetComponent<ElevatorBrain>();
                brain.elevator = elevator.transform;
                brain.player = player;

                
                //a handy static reference to this elevator
                if (elevatorIndex == 0) ElevatorBrain.elevators[elevatorIndex] = brain;

                //initialize switch references for this elevator
                brain.switches = new DoorSwitch[howManyFloors];

                //floor detection
                elevator.GetComponentInChildren<ElevatorFloorDetection>().brain = brain;


                //elevator lights
                brain.elevatorLightsContainer = 
                    (elevator.transform.Find("Lights") ?? elevator.transform.Find("ElevatorLights")).gameObject;
                SetupLights(elevator.transform, elevatorLightsType, elevatorLightsAmount);


                //set the inner doors up
                foreach (AutomaticDoor door in elevator.GetComponentsInChildren<AutomaticDoor>()) {
                    door.doorLocation = AutomaticDoor.DoorLocation.Internal;
                    door.brain = brain;
                    door.floorStop = null;
                    door.locked = false;
                }



                //elevator mirror 
                bool mirrorOnElevatorBackWall = false; //not in built-in for now
                elevator.transform.Find("Mirror").gameObject.SetActive(mirrorOnElevatorBackWall);
                elevator.transform.Find("Back panel").gameObject.SetActive(!mirrorOnElevatorBackWall);
                

                //stops container
                Transform stopsContainer = new GameObject("Stops").transform;
                stopsContainer.parent = brainTransform;
                stopsContainer.localPosition = Vector3.zero;


                //ref. to audio sources
                Transform soundsContainer = elevator.transform.Find("Sounds");


                if (soundsContainer != null) {
                    Transform tempContainer = soundsContainer.GetChild(0);
                    brain.travelSound = tempContainer?.GetChild(0)?.GetComponent<AudioSource>();
                    brain.stopSound = tempContainer?.GetChild(1)?.GetComponent<AudioSource>();

                    tempContainer = soundsContainer.GetChild(1);
                    brain.arrived_ding = tempContainer?.GetChild(0)?.GetComponent<AudioSource>();
                    brain.arrived_Arp3NotesLow = tempContainer?.GetChild(1)?.GetComponent<AudioSource>();
                    brain.arrived_Arp3NotesHigh = tempContainer?.GetChild(2)?.GetComponent<AudioSource>();
                    brain.arrived_Arp2NotesHigh = tempContainer?.GetChild(3)?.GetComponent<AudioSource>();

                    tempContainer = soundsContainer.GetChild(2);
                    brain.doorOpensSound = tempContainer?.GetChild(0)?.GetComponent<AudioSource>();
                    brain.doorClosesSound = tempContainer?.GetChild(1)?.GetComponent<AudioSource>();

                    tempContainer = soundsContainer.GetChild(3);
                    for (int i = 0; i < 3; i++) {
                        brain.flickeringSounds[i] = tempContainer?.GetChild(i)?.GetComponent<AudioSource>();
                    }

                }



                //building
                Transform building = new GameObject("Building").transform;
                building.parent = thisContainer;
                building.localPosition = Vector3.zero;


                //elevator pit (cases container)
                Transform pit = new GameObject("Elevator pit").transform;
                pit.parent = building;
                pit.localPosition = Vector3.zero;


                //elevator pit (cases container)
                Transform frontContainer = new GameObject("Halls and fronts").transform;
                frontContainer.parent = building;
                frontContainer.localPosition = Vector3.zero;

                //bottom case
                Transform bottomCase = InstPrefab(refs.bottomCase, y - 3f, 0, pit).transform;
                SetShadowCasters(bottomCase);


                //buttons pad layout
                float _verticalSpaceBetweenButtons = verticalSpaceBetweenButtons; //so the original remains unchanged
                float horizontalSpaceBetweenButtons = 0;
                float xButtons = 0;
                int columns = 1;

                switch (buttonsLayout) {
                    case ButtonsLayout.Auto:
                        if (howManyFloors > 9) columns = 2;
                        if (howManyFloors > 20) columns = 3;
                        break;

                    case ButtonsLayout.TwoColumns:
                        columns = 2;
                        break;

                    case ButtonsLayout.ThreeColumns:
                        columns = 3;
                        break;
                }

                switch (columns) {
                    case 1:
                        xButtons = 0;
                        break;
                    case 2:
                        xButtons = 0.35f;
                        horizontalSpaceBetweenButtons = .9f;
                        break;
                    case 3:
                        horizontalSpaceBetweenButtons = 0.76f;
                        xButtons = 0 + horizontalSpaceBetweenButtons;
                        break;

                }

                if (howManyFloors > 23) _verticalSpaceBetweenButtons *= .95f;
                if (howManyFloors > 27) _verticalSpaceBetweenButtons *= .9f;

                Transform buttonsArea = elevator.transform.Find("Buttons Panel/Buttons Container");
                float rows = Mathf.FloorToInt((howManyFloors - 1f) / columns) + 1;
                float yButtons = -2f - _verticalSpaceBetweenButtons * ((rows-1) / 2f);

                //special buttons
                buttonsArea.Find("Alarm Button").gameObject.SetActive(addAlarmButton);
                buttonsArea.Find("Lights Button").gameObject.SetActive(addLightsButton);





                //__________________________ PART 3: EACH FLOOR ____________________________

                Transform thisHall;

                for (int floorIndex = 0; floorIndex < howManyFloors; floorIndex++) {

                    centerOfThisFloor = generalContainer.position + new Vector3(centerX, 0, 3f);

                    int floorNumber = floorIndex + firstFloorNumber;
                    floorNumber1stDigit = int.Parse(floorNumber.ToString("00").Substring(0, 1));
                    floorNumber2ndDigit = floorNumber % 10;


                    //save this position
                    Transform thisStop = new GameObject(floorNumber.ToString()).transform;
                    thisStop.parent = stopsContainer;
                    thisStop.localPosition = new Vector3(0, y, 0);

                    if (floorIndex == 0) brain.initialStop = thisStop;

                    Transform thisFloorContainer;
                    if (floorIndex == 0 && lobbyAsFirstFloor) {
                        //add a middle piece in the lobby
                        thisFloorContainer = InstPrefab(refs.lobbyMiddle,
                            new Vector3(elevatorIndex * deltaX, 0, 0),
                            Quaternion.identity,
                            lobby
                        ).transform;

                    } else {
                        //only when this floor is not the lobby:

                        //a container for this floor
                        thisFloorContainer = new GameObject("Floor " + floorNumber.ToString()).transform;
                        thisFloorContainer.parent = frontContainer;
                        thisFloorContainer.localPosition = new Vector3(0, y, 0);

                        float z = 0;
                        if (addDemoHalls) {

                            //add hall and rooms
                            thisHall = InstPrefab(
                                refs.hallA,
                                new Vector3(0, 0, z),
                                Quaternion.identity,
                                thisFloorContainer,
                                1f,
                                "Hallway"
                            ).transform;


                            //if there is another elevator to the right, remove right wall
                            Transform side = thisHall.Find("Right side");
                            if (elevatorIndex < howManyElevators - 1)
                                side?.gameObject.SetActive(false);
                            else {
                                ApplyFancyNumbersSetupToWall(side, numbersTypeInHallWalls, floorNumber);
                            }


                            //if there is another elevator to the left, remove left wall
                            side = thisHall.Find("Left side");
                            if (elevatorIndex > 0)
                                side?.gameObject.SetActive(false);
                            else {
                                ApplyFancyNumbersSetupToWall(side, numbersTypeInHallWalls, floorNumber);
                            }

                            //add the back wall
                            if (addBackWallInHalls) {
                                InstPrefab(
                                    refs.hallBackWall,
                                    new Vector3(0, 0, z - 9.36f),
                                    Quaternion.identity,
                                    thisFloorContainer
                                );
                            }

                            //floor lights
                            SetupLights(thisHall, hallsLightsType);

                            //refl. probe in hall (only once per floor)
                            if (elevatorIndex == 0 && addReflectionProbesInOthers) {
                                InstPrefab(refs.reflectionProbe, centerOfThisFloor, Quaternion.identity, thisHall);
                            }


                        }


                    }
                    SetShadowCasters(thisFloorContainer);

                    //entrance case (without front)
                    Transform entranceCaseTr = InstPrefab(refs.entranceCase, y, 0, pit).transform;
                    SetShadowCasters(entranceCaseTr);


                    //set the external doors up
                    foreach (AutomaticDoor door in entranceCaseTr.GetComponentsInChildren<AutomaticDoor>()) {
                        door.doorLocation = AutomaticDoor.DoorLocation.External;
                        door.brain = brain;
                        door.floorStop = thisStop;
                        door.forced = false; //...yet ;)
                    }


                    //set (or hide) the painted number in the pit
                    SpriteRenderer sr =
                        entranceCaseTr.Find("Big number painted in pit wall")
                        .GetComponent<SpriteRenderer>();

                    if (addFloorNumbersInPitWalls) {
                        NumberInSprite(sr, floorNumber, .9f, refs.rustyNumbersInPit);
                    }
                    else {
                        sr.gameObject.SetActive(false);
                    }


                    //set the switch
                    DoorSwitch _switch = entranceCaseTr.GetComponentInChildren<DoorSwitch>();
                    _switch.brain = brain;
                    _switch.floor = thisStop;
                    brain.switches[floorIndex] = _switch;


                    //reduced lights in pit? Turn off entrance case lights, just leave the inner cases' on
                    if (pitLightsAmount == LightsAmount.Reduced) SetupLights(entranceCaseTr, LightsType.NoLights);


                    //should the caller be moved to the left side?
                    if (
                        callerSide == CallerSide.AlwaysLeft ||
                        (callerSide == CallerSide.MirroredInner && elevatorIndex % 2 == 1) ||
                        (callerSide == CallerSide.MirroredOuter && elevatorIndex % 2 == 0)
                    ) entranceCaseTr.Find("Caller").Translate(1.94f, 0, 0, Space.Self);


                    //add front 
                    Transform front = null;
                    FrontType frontTypeThisFloor = (floorIndex==0 && lobbyAsFirstFloor) ? frontTypeLobby : frontTypeOtherFloors;

                    if (frontTypeThisFloor == FrontType.LightMarble)
                        front = InstPrefab(refs.frontLightMarble, 0, -90, thisFloorContainer, 100).transform;

                    if (frontTypeThisFloor == FrontType.DarkMarble)
                        front = InstPrefab(refs.frontDarkMarble, 0, -90, thisFloorContainer, 100).transform;


                    //show the double wall only if there is another elevator to the left
                    Transform widePanel = front?.Find("Wide wall panel");

                    if (widePanel != null) {
                        widePanel.gameObject.SetActive(elevatorIndex > 0);

                        //fancy numbers in front wide wall
                        if (elevatorIndex > 0) {
                            ApplyFancyNumbersSetupToWall(widePanel, numbersTypeInFront, floorNumber);
                        }
                    }


                    //if it's not the last hall, add a middle case above
                    if (floorIndex != howManyFloors - 1) {
                        float currentDistanceBetweenFloors =
                            (floorIndex == 0)
                            ? distanceBetweenLobbyAnd2ndFloor
                            : distanceBetweenFloors;

                        Transform thisCase = InstPrefab(
                            refs.innerCase,
                            y + 1.5f + (currentDistanceBetweenFloors / 2),
                            -0,
                            pit
                        ).transform;

                        //stretch or shrink inner case 
                        thisCase.localScale = new Vector3(
                            thisCase.localScale.x,
                            thisCase.localScale.y / 3f * currentDistanceBetweenFloors,
                            thisCase.localScale.z
                        );

                        SetupLights(thisCase, pitLightsType);
                        SetShadowCasters(thisCase);



                        y += 3 + currentDistanceBetweenFloors;
                    }


                    //hide the buttons on this caller if this floor is first/last
                    entranceCaseTr.Find("Caller/Button Up").gameObject.SetActive(floorIndex < howManyFloors - 1);
                    entranceCaseTr.Find("Caller/Button Down").gameObject.SetActive(floorIndex > 0);



                    //add a button in the pad

                    Vector3 thisBtnAndNumPos = Vector3.zero;
                    if (columns == 1) {
                        thisBtnAndNumPos = new Vector3(xButtons, yButtons, 0);
                        yButtons += _verticalSpaceBetweenButtons;
                    }
                    else {
                        if (floorIndex % columns == 0) {
                            //first button in row
                            yButtons += _verticalSpaceBetweenButtons;
                            thisBtnAndNumPos = new Vector3(xButtons, yButtons, 0);
                        }
                        else {
                            //other
                            thisBtnAndNumPos = new Vector3(
                                xButtons - horizontalSpaceBetweenButtons * (floorIndex % columns),
                                yButtons,
                                0
                            );

                        }
                    }


                    GameObject thisBtnAndNum = InstPrefab(
                        refs.buttonAndNumber,
                        thisBtnAndNumPos,
                        Quaternion.Euler(0, 90f, 0),
                        buttonsArea,
                        10f
                    );


                    //tell the pad button where to send the elevator
                    ElevatorButton thisButton = thisBtnAndNum.GetComponentInChildren<ElevatorButton>();
                    thisButton.stop = thisStop;
                    thisButton.isCallerButton = false;


                    //change the button label
                    NumberInSprite(
                        thisBtnAndNum.GetComponentInChildren<SpriteRenderer>(),
                        floorNumber,
                        .0075f,
                        refs.fancyNumbersInWalls
                    );


                    //add braille number to the plate
                    Transform brailleCont =  thisBtnAndNum.transform.Find(
                        "PlateButtonInterior/Braille scaled container"
                    );
                    if (brailleCont != null) {
                        InstPrefab(
                            refs.brailleNumbersPrefabs[floorNumber % 10],
                            new Vector3(0, .09f, 0),
                            Quaternion.identity,
                            brailleCont
                        );
                    }


                    //and also tell both buttons of the caller in this floor where to send the elevator
                    foreach (ElevatorButton btn in entranceCaseTr.GetComponentsInChildren<ElevatorButton>()) {
                        if (btn.buttonType == ElevatorButton.ButtonType.GoTo) btn.stop = thisStop;
                        btn.isCallerButton = true;
                    }
                }


                //__________________________ PART 3: WRAPPING UP __________________________

                //finally, the top case
                Transform topCaseTr = InstPrefab(refs.topCase, y + 3f, 0, pit).transform;
                SetShadowCasters(topCaseTr);

                //elevator wires
                for (int i = 0; i < 4; i++) {
                    brain.wireEndBone[i] = elevator.transform.Find($"Machinery/Wires/{i}/Armature/End");
                    brain.wiresEndPosition[i] = new Vector3(
                        brain.wireEndBone[i].position.x,
                        topCaseTr.transform.position.y + 1.35f,
                        brain.wireEndBone[i].position.z
                    );
                }

                brain.UpdateWires(); //stretch the wire to reach the top engine


                //tell every door, button and display to obbey to this elevator
                foreach (AutomaticDoor door in thisContainer.GetComponentsInChildren<AutomaticDoor>()) {
                    door.brain = brain;
                }
                foreach (ElevatorButton btn in thisContainer.GetComponentsInChildren<ElevatorButton>()) {
                    btn.brain = brain;
                }
                foreach (DigitalDisplay display in thisContainer.GetComponentsInChildren<DigitalDisplay>()) {
                    display.brain = brain;
                }

                //lobby lights
                if (lobbyAsFirstFloor) {
                    for (int i = 0; i < lobby.childCount; i++) {
                        SetupLights(lobby.transform.GetChild(i), lobbyLightsType, lobbyLightsAmount);
                    }
                }

                //pit lights (only type, not amount)
                SetupLights(pit, pitLightsType);



            } //next elevator


            //count lights
            int countBaked = 0, countRealTime = 0, countMixed = 0;
            foreach (Light l in generalContainer.GetComponentsInChildren<Light>()) {
                if (l.gameObject.activeSelf && l.enabled) {
                    if (l.lightmapBakeType == LightmapBakeType.Baked) countBaked++;
                    if (l.lightmapBakeType == LightmapBakeType.Realtime) countRealTime++;
                    if (l.lightmapBakeType == LightmapBakeType.Mixed) countMixed++;
                }
            }


            //finally, restore builder rotation
            transform.rotation = originalBuilderRotation;

            Debug.Log(
                $"Building finished successfully! \r\n" +
                $"Lights count: {countRealTime} realtime, {countBaked} baked, {countMixed} mixed. "
            );

        }

        void SetShadowCasters(Transform parent) {
            if (parent != null) {
                parent.Find("Shadow casters")?.gameObject.SetActive(addShadowCasters);
            }
        }

        void NumberInSprite(SpriteRenderer sr, int num, float distBetweenDigits, Sprite[] numbersSet) {

            sr.sprite = numbersSet[floorNumber2ndDigit];
            if (forceTwoDigitsInFloorNumbers || floorNumber1stDigit > 0) {
                //move the original sprite a little to the right
                Transform orig = sr.gameObject.transform;
                Vector3 origPos = orig.position;
                orig.position += orig.right * (distBetweenDigits / 2f);
                orig.localScale *= .7f;

                //make a copy of the same object
                Transform firstDigit = Instantiate(
                    sr.gameObject,
                    orig.position,
                    orig.rotation,
                    orig.parent
                ).transform;

                firstDigit.position = origPos - firstDigit.right * (distBetweenDigits / 2f);

                firstDigit.name = orig.name + " - " + floorNumber1stDigit.ToString();
                firstDigit.GetComponent<SpriteRenderer>().sprite = numbersSet[
                    floorNumber1stDigit
                ];

            }
            sr.gameObject.name += " - " + (num % 10).ToString();

        }



        void ApplyFancyNumbersSetupToWall(Transform wall, NumbersType numbersStyle, int floorNumber) {
            bool twoDigits = forceTwoDigitsInFloorNumbers || (floorNumber1stDigit > 0);

            Transform cont_1dig = wall.Find("Number in wall container/Number in wall (1 digit)");
            Transform cont_2dig = wall.Find("Number in wall container/Number in wall (2 digits)");
            Transform cont_met  = wall.Find("Number in wall container/Number in wall (metallic)");
            Transform cont_marble=wall.Find("Number in wall container/Number in wall (marble)");

            cont_1dig.gameObject.SetActive(numbersStyle != NumbersType.Classic && !twoDigits);
            cont_2dig.gameObject.SetActive(numbersStyle != NumbersType.Classic && twoDigits);
            cont_met.gameObject.SetActive(numbersStyle == NumbersType.Classic);
            cont_marble.gameObject.SetActive(numbersStyle == NumbersType.LightMarble || numbersStyle == NumbersType.DarkMarble);

            switch (numbersStyle) {
                case NumbersType.Mirror:

                    if (!twoDigits) {
                        InstantiateAndApplyNumber(cont_1dig, floorNumber2ndDigit, numbersStyle);
                    } else {
                        InstantiateAndApplyNumber(cont_2dig.GetChild(0), floorNumber1stDigit, numbersStyle);
                        InstantiateAndApplyNumber(cont_2dig.GetChild(1), floorNumber2ndDigit, numbersStyle);
                    }
                    break;

                case NumbersType.LightMarble:
                    NumberInSprite(cont_marble.GetChild(0).GetComponent<SpriteRenderer>(), floorNumber, .35f, refs.fancyNumbersInWalls);
                    break;

                case NumbersType.DarkMarble:
                    NumberInSprite(cont_marble.GetChild(0).GetComponent<SpriteRenderer>(), floorNumber, .35f, refs.fancyNumbersInWalls);
                    cont_marble.GetComponent<MeshRenderer>().material = refs.darkMarbleForPlaques;
                    break;

                case NumbersType.Classic:
                    GameObject firstClassic = InstPrefab(
                        refs.metallicNumbersPrefabs[floorNumber2ndDigit],
                        new Vector3(-.17f, 0, 0),
                        Quaternion.identity,
                        cont_met
                    );
                    if (twoDigits) {
                        firstClassic.transform.position -= new Vector3(.25f, 0, 0);
                        InstPrefab(
                            refs.metallicNumbersPrefabs[floorNumber1stDigit],
                            new Vector3(.17f + -.15f, 0, 0),
                            Quaternion.identity,
                            cont_met
                        );
                    }
                    break;
            }

            void InstantiateAndApplyNumber(Transform parent, int number, NumbersType numbersType) {
                parent.gameObject.SetActive(true);

                GameObject prefabToApply = null;
                switch (numbersType) {
                    case NumbersType.Mirror:
                        prefabToApply = twoDigits ? refs.numberInWallMirrorHalf : refs.numberInWallMirror;
                        break;
                    case NumbersType.LightMarble:
                        prefabToApply = twoDigits ? refs.numberInWallMirrorHalf : refs.numberInWallMarble;
                        break;
                    case NumbersType.Classic:

                        break;
                }

                Transform spriteAsChild = parent.Find("number sprite"); //probably null
                switch (numbersType) {
                    case NumbersType.Classic:
                        //nothing
                        break;

                    case NumbersType.Mirror:
                        //the number is set as the material's normal map
                        GameObject newNumber = InstPrefab(prefabToApply, 0, 0, parent);

                        MeshRenderer mr = newNumber.GetComponent<MeshRenderer>();

                        Material tempMaterial = new Material(mr.sharedMaterial);
                        mr.material = tempMaterial;

                        tempMaterial.EnableKeyword("_NormalMap");
                        tempMaterial?.SetTexture("_NormalMap", refs.fancyNumbersInWalls[number].texture);

                        if (spriteAsChild != null) spriteAsChild.gameObject.SetActive(false);
                        
                        break;

                    case NumbersType.LightMarble:
                        //the number is set as a child sprite
                        if (spriteAsChild == null) {
                            Debug.LogWarning("Type marble, without child sprite", parent);
                            return;
                        }

                        spriteAsChild.gameObject.SetActive(true);
                        spriteAsChild.GetComponent<SpriteRenderer>().sprite = refs.fancyNumbersInWalls[number];
                        break;
                }
            }
        }

        [ContextMenu("✗   Clear all")]
        public void ClearAll() {
            for (int i = transform.childCount - 1; i > -1; i--) {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
            Debug.Log("All buildings cleared!");
        }

        [ContextMenu("↳    Detach")]
        public void Detach() {
            if (transform.childCount > 0) {
                Transform elevator = transform.GetChild(0);
                elevator.parent = null;
                Debug.Log("'" + elevator.name + "' successfully detached", elevator);
            }
            else {
                Debug.LogWarning("You have to build first in order to detach the elevator", gameObject);
            }

        }

        [ContextMenu("✗▶ Clear and Build")]
        public void ClearAndBuild() {
            ClearAll();
            Build();
        }


        GameObject InstPrefab(GameObject original, float yPos, float xRot, Transform parent, float scale = 1, string name = "") {
            return InstPrefab(original, new Vector3(0, yPos, 0), Quaternion.Euler(xRot, 0, 0), parent, scale, name);
        }

        GameObject InstPrefab(GameObject original, Vector3 pos, Quaternion rot, Transform parent, float scale = 1, string name = "") {

            GameObject go =
                instantiateMethod == InstantiateMethod.PlacePrefabs
                ? PrefabUtility.InstantiatePrefab(original, parent) as GameObject
                : Instantiate(original, parent); 
            
            go.transform.localPosition = pos;
            go.transform.localRotation = rot;
            go.transform.localScale = new Vector3(scale, scale, scale);
            if (name != "") go.name = name;
            return go;
        }

        void SetupLights(Transform container, LightsType lightsType, LightsAmount? amount = null) {
            foreach (Light light in container.GetComponentsInChildren<Light>()) {
                switch (lightsType) {
                    case LightsType.NoLights:
                        light.gameObject.SetActive(false);
                        break;
                    case LightsType.Realtime:
                        light.lightmapBakeType = LightmapBakeType.Realtime;
                        break;
                    case LightsType.Baked:
                        light.lightmapBakeType = LightmapBakeType.Baked;
                        break;
                    case LightsType.Mixed:
                        light.lightmapBakeType = LightmapBakeType.Mixed;
                        break;
                }

                if (amount != null) {
                    Transform tempFolder = container.Find("Lights/Full") ?? (container.Find("Lights")?.GetChild(0));
                    tempFolder?.gameObject.SetActive(amount == LightsAmount.Full);

                    tempFolder = container.Find("Lights/Reduced") ?? (container.Find("Lights")?.GetChild(1));
                    tempFolder?.gameObject.SetActive(amount == LightsAmount.Reduced);
                }
                    
            }
        }

    }
}
#endif