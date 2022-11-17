using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Main : MonoBehaviour
{
    enum Level { LEVELZERO, LEVELONE, LEVELTWO, LEVELTHREE };
    enum GameStatus { LOAD, RUNNING, WON, LOST };

    public GameObject player;               // internal player object
    public GameObject leftHand;             // left controller
    public GameObject rightHand;            // right controller
    public GameObject lose;                 // assets in transition room when player loses game
    public GameObject win;                  // assets in transition room when player wins game
    public GameObject nextLevel;            // assets in transition room when player is going to next level
    public GameObject restartLevel;         // assets in transition room when player is restarting level
    Level currLevel;                        // the current level being played
    GameStatus status;                      // status of the game, if it's running, won, or lost
    string hand;                            
    bool internalActive;                    // controller for the internal player (left controller)
    bool externalActive;                    // controller for the external player (right controller)
    bool hasLoaded;                         // make sure the whole scene has loaded
    bool hasWon;                            // check if the internal player has won or lost
    Vector3 transportPlayer;                // Vector that holds position that player should transport to
    GameObject uiObj;
    UIManager ui;
    bool isReady;                           // flag that makes sure player is ready to go to next level
    bool inTransition;                      // if player is in transition room 
    public GameObject instrObj1;            
    public GameObject instrObj2;
    public GameObject instrObj3;
    AudioSource level1Instr;          // audio for instructions on how to play level 1
    AudioSource level2Instr;          // audio for instructions on how to play level 2
    AudioSource level3Instr;          // audio for instructions on how to play level 3
    public GameObject repeat;         // GameObject to interact with if player wants to hear instructions again
    public GameObject playerReady;    // GameObject to interact with if player is ready to play next level
    bool selectedButton;              // make sure player has selected an option before trying to spawn object

    // timer for each level
    float startTime;                        //level start time - resets every time a new level loads
    
    // level 1 vars
    public float rotationSpeed;             // speed to rotate the room for level 1
    GameObject room;                        // gameobject refers to the entire level 1 room (to rotate)
    public GameObject visibleDoor;          // gameobject refers to visible door in level 1
    GameObject targetDoor;                  // gameobject refers to original position of door in level 1
    public float thresholdDistance;         // distance door has to be from target position to be considered "winning"
    float doorVisibleTime;                  // total time door is visible when level 1 starts before disappearing
    public GameObject internalPlayerPopUp;   // pop up showing internal player time to play
    public GameObject externalPlayerPopUp;   // pop up showing external player time to play
    public GameObject levelEndedObject;     // pop up showing level has ended
    float popUpStart;                       // time pop up starts
    float popUpDuration;                    // total time pop up should be visible
    GameObject[] keys;                      // array that holds the keys to find in level 1
    int totalKeys;                          // total keys to be found in level 1
    bool keyEnabled;                        // boolean to tell if a key is active in level 1
    public GameObject currentEnabledKey;    // the current key that is active for level 1
    int keysHeld;                           // number of keys held (number of lives player has)
    bool findKeys;                          // bool that checks if the hidden keys in level 1 have been identified by Unity
    bool levelEnded;                         // check if the level has ended
    [SerializeField]
    private float levelOneTotalDuration;    // duration of running state for level 1
    public float levelOnePlayerDuration;    // duration of internal player for level 1
    public float levelOneDemonDuration;     // duration of external player for level 1
    float controllerThreshold;              // distance controller has to be to collect object

    // level 2 vars
    public int numKeys;                     // number of keys collected in level 2
    public GameObject spawnObject;          // hidden object for external player to spawn in level 2
    private GameObject recentCube;          // the most recent object that was spawned in level 2
    bool pressed;                           // check if the trigger button on the controller is being pressed
    [SerializeField]
    private float levelTwoTotalDuration;    // duration of running state for level 2
    public float levelTwoPlayerDuration;    // duration of internal player for level 2
    public float levelTwoDemonDuration;     // duration of external player for level 2
    public List<GameObject> objectsPlaced;      // holds the objects placed by the external player  

    // level 3 vars
    public GameObject audioObject;          // audio object that gets instantiated
    public AudioSource audioSource;         // audio source attached to audio object
    public GameObject spawnAudio;           // the gameobject of the audioobject to spawn in level 3           
    public bool musicActive;                // check if music is playing in level 3
    public float followSpeed;               // the speed at which the audio object should follow the external controller
    public int musicCollected;              // the number of different audios/sounds the internal player has collected
    public float soundDist;                 // the threshold distance for the internal player to interact/pick up the sound 
    public float levelThreeTotalDuration;   // duration of running state for level 3


    // Start is called before the first frame update
    void Start()
    {
        player.transform.position = new Vector3(-11.87f, 2.71f, 19.01f);    // comment back in
        leftHand = GameObject.Find("LeftHand");
        rightHand = GameObject.Find("RightHand");
        internalActive = false;              
        externalActive = false;              
        hasWon = true;
        hasLoaded = false;
        transportPlayer = new Vector3(-11.87f, 2.71f, 19.01f);
        uiObj = GameObject.Find("ScreenUI");
        ui = uiObj.GetComponentInChildren<UIManager>();
        isReady = false;
        inTransition = true;                // change to true
        currentEnabledKey = null;
        win.SetActive(false);
        lose.SetActive(false);
        restartLevel.SetActive(false);
        nextLevel.SetActive(true);
        level1Instr = instrObj1.GetComponent<AudioSource>();
        level2Instr = instrObj2.GetComponent<AudioSource>();
        level3Instr = instrObj3.GetComponent<AudioSource>();
        selectedButton = false;

        SceneManager.LoadScene("SpinningRoom", LoadSceneMode.Additive);      // change to SpinningRoom
        levelOneTotalDuration = levelOnePlayerDuration + levelOneDemonDuration;
        popUpDuration = 5f;
        doorVisibleTime = 10f;
        startTime = 0;
        currLevel = Level.LEVELZERO;            // change to ZERO
        status = GameStatus.LOAD;            // change to LOAD

        // ----- set level 1 vars
        keyEnabled = false;
        keysHeld = 0;
        totalKeys = 0;
        findKeys = false;
        controllerThreshold = 0.1f;
 
        rotationSpeed = 5f;
        thresholdDistance = .5f;
        internalPlayerPopUp.SetActive(false);
        externalPlayerPopUp.SetActive(false);
        levelEndedObject.SetActive(false);
        keys = new GameObject[5];

        // ----- set level 2 vars
        numKeys = 0;
        pressed = false;
        levelEnded = false;
        levelTwoTotalDuration = levelTwoPlayerDuration + levelTwoDemonDuration;

        // ----- set level 3 vars
        musicActive = false;
        followSpeed = 0.15f;
        musicCollected = 0;
        soundDist = 0.5f;
    }




    // Update is called once per frame
    void Update()
    {
        Debug.Log(currLevel);
        int level = getLevel();
        ui.SetLevelMenu(level);
        if (!hasLoaded)
        {
            hasLoaded = true;
            return;
        }

        switch (currLevel)
        {
            case Level.LEVELZERO:
                if (isReady)
                {                
                    status = GameStatus.RUNNING;
                    currLevel = Level.LEVELONE;
                    startTime = Time.time;
                    externalPlayerPopUp.SetActive(true);
                    player.transform.position = Vector3.zero;
                    isReady = false;
                }
                break;
            case Level.LEVELONE:                
                switch (status)
                {
                    case GameStatus.RUNNING:                        
                        updateLevelOne();
                        break;
                    case GameStatus.WON:
                        // load next level scene
                        if (isReady)     // going to next level
                        {
                            inTransition = false;
                            isReady = false;
                            player.transform.position = Vector3.zero;
                            currLevel = Level.LEVELTWO;
                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            //internalActive = false;
                            //externalActive = true;
                            levelEnded = false;
                            levelEndedObject.SetActive(false);
                        }
                        break;
                    case GameStatus.LOST:                        
                        // transitioning back to next level
                        if (isReady)
                        {
                            inTransition = false;
                            isReady = false;
                            player.transform.position = Vector3.zero;
                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            //internalActive = true;
                            //externalActive = true;
                            levelEnded = false;
                            findKeys = false;
                            for (int i = 0; i < keys.Length; i++)
                            {
                                keys[i] = null;
                            }
                            levelEndedObject.SetActive(false);
                        }
                        break;
                    default:
                        break;
                }
                break;
            case Level.LEVELTWO:
                switch (status)
                {
                    case GameStatus.RUNNING:
                        updateLevelTwo();
                        break;
                    case GameStatus.WON:
                        if (isReady)     // players transitioning to next level
                        {
                            inTransition = false;
                            isReady = false;
                            player.transform.position = Vector3.zero;
                            startTime = Time.time;
                            currLevel = Level.LEVELTHREE;
                            status = GameStatus.RUNNING;
                            //internalActive = true;
                            //externalActive = true;
                            levelEnded = false;
                            levelEndedObject.SetActive(false);
                            selectedButton = false;
                        }
                        break;
                    case GameStatus.LOST:
                        if (isReady)     // players restarting level
                        {
                            inTransition = false;
                            isReady = false;
                            player.transform.position = Vector3.zero;
                            // destroy all the objects that were spawned by external player
                            for (int i = 0; i < objectsPlaced.Count; i++)
                            {
                                Destroy(objectsPlaced[i]);
                            }
                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            //internalActive = true;
                            //externalActive = true;
                            levelEnded = false;
                            levelEndedObject.SetActive(false);
                            selectedButton = false;
                        }
                        break;
                    default:
                        break;
                }
                break;
            case Level.LEVELTHREE:
                switch (status)
                {
                    case GameStatus.RUNNING:
                        updateLevelThree();
                        break;
                    case GameStatus.WON:                       
                        if (isReady)
                        {
                            inTransition = false;
                            isReady = false;
                            player.transform.position = Vector3.zero;
                            if (musicActive)
                            {
                                Destroy(audioObject);
                                musicActive = false;
                            }
                            startTime = Time.time;
                            currLevel = Level.LEVELTHREE;
                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            //internalActive = true;
                            //externalActive = true;
                            levelEnded = false;
                            levelEndedObject.SetActive(false);
                        }
                        break;
                    case GameStatus.LOST:
                        // transitioning back to next level
                        if (isReady)
                        {
                            inTransition = false;
                            isReady = false;
                            player.transform.position = Vector3.zero;
                            if (musicActive)
                            {
                                Destroy(audioObject);
                                musicActive = false;
                            }
                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            //internalActive = true;
                            //externalActive = true;
                            levelEnded = false;
                            levelEndedObject.SetActive(false);
                            selectedButton = false;
                        }
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    /**
     * Method that detects any collisions the player makes using their controller
     */
    public void handCollided(string h, GameObject obj)
    {
        if (currLevel == Level.LEVELZERO)
        {
            Debug.Log(level1Instr.clip);
            // repeat the instructions for level one
            if (inTransition && obj.tag == "Repeat")
            {
                Debug.Log("repeating instructions for level 1");
                AudioClip audio1 = level1Instr.clip;
                if (audio1 != null && !level1Instr.isPlaying)
                {
                    level1Instr.PlayOneShot(audio1);
                }
            }
            if (inTransition && obj.tag == "Ready")
            {
                isReady = true;
            }
        }
        if (currLevel == Level.LEVELONE)
        {
            // check if internal player touched key
            if (h == "LeftHand" && obj.gameObject.tag == "Key" && !internalActive)
            {
                Debug.Log("TOUCHED KEY");
                Destroy(obj);
                keysHeld++;
                keyEnabled = false;
            }

            // repeat the instructions for level one
            else if (inTransition && obj.tag == "Repeat")
            {
                // repeat instructions for level 2 
                if (hasWon)
                {
                    Debug.Log("repeating instructions for level 2");
                    AudioClip audio2 = level2Instr.clip;
                    if (audio2 != null && !level2Instr.isPlaying)
                    {
                        level2Instr.PlayOneShot(audio2);
                    }
                }
                // repeat instructions for level 1
                else
                {
                    Debug.Log("repeating instructions for level 1");
                    AudioClip audio1 = level1Instr.clip;
                    if (audio1 != null && !level1Instr.isPlaying)
                    {
                        level1Instr.PlayOneShot(audio1);
                    }
                }
            }
            // check if player is ready to go to the next level 
            else if (inTransition && obj.tag == "Ready")
            {
                isReady = true;
            }
            else if (levelEnded)
            {
                // check if player collided with door, move to transition room
                if (h == "LeftHand" && obj.gameObject.tag == "Door")
                {
                    Debug.Log("collided with door!");
                    if (hasWon)
                    {
                        status = GameStatus.WON;
                        LoadNextLevel("SpinningRoom", "ClutterRoom");
                        player.transform.position = transportPlayer;
                        win.SetActive(false);
                        lose.SetActive(false);
                        restartLevel.SetActive(false);
                        nextLevel.SetActive(true);
                        AudioClip audio2 = level2Instr.clip;
                        if (audio2 != null && !level2Instr.isPlaying)
                        {
                            level2Instr.PlayOneShot(audio2);
                        }
                        inTransition = true;
                        Debug.Log("player has won level! Loading next level");
                        internalActive = false;
                        externalActive = false;
                        levelEndedObject.SetActive(false);
                    }
                    else
                    {
                        status = GameStatus.LOST;
                        keysHeld--;
                        inTransition = true;
                        player.transform.position = transportPlayer;
                        // check how many lives player has left, if <= 0, transport to "lose" room
                        if (keysHeld <= 0)
                        {
                            // turn off "transition" room and turn on "lose" room
                            win.SetActive(false);
                            lose.SetActive(true);
                            restartLevel.SetActive(false);
                            nextLevel.SetActive(false);
                            Debug.Log("player has lost game!");
                        }
                        else
                        {
                            // player restarts level
                            win.SetActive(false);
                            lose.SetActive(false);
                            restartLevel.SetActive(true);
                            nextLevel.SetActive(false);
                            AudioClip audio1 = level1Instr.clip;
                            if (audio1 != null && !level1Instr.isPlaying)
                            {
                                level1Instr.PlayOneShot(audio1);
                            }
                            Debug.Log("player has lost level! Reloading level");
                            ReloadLevel("SpinningRoom");
                            internalActive = false;
                            externalActive = false;
                            levelEndedObject.SetActive(false);
                        }

                    }
                }
            }
            
        }
        else if (currLevel == Level.LEVELTWO)
        {
            if (internalActive)
            {
                if (h == "LeftHand" && obj.gameObject.tag == "Key")
                {
                    Debug.Log("TOUCHED KEY");
                    Destroy(obj);
                    numKeys++;
                }
            }
            else if (inTransition && obj.tag == "Repeat")
            {
                // repeat instructions for level 3 
                if (hasWon)
                {
                    Debug.Log("repeating instructions for level 3");
                    AudioClip audio3 = level3Instr.clip;
                    if (audio3 != null && !level3Instr.isPlaying)
                    {
                        level3Instr.PlayOneShot(audio3);
                    }
                }
                // repeat instructions for level 2
                else
                {
                    Debug.Log("repeating instructions for level 2");
                    AudioClip audio2 = level2Instr.clip;
                    if (audio2 != null && !level2Instr.isPlaying)
                    {
                        level2Instr.PlayOneShot(audio2);
                    }
                }
            }
            // check if player is ready to go to the next level 
            else if (inTransition && obj.tag == "Ready")
            {
                isReady = true;
            }
            else if (levelEnded)
            {
                // check if player collided with door, move to transition room
                if (h == "LeftHand" && obj.gameObject.tag == "Door")
                {
                    if (hasWon)
                    {
                        status = GameStatus.WON;
                        player.transform.position = transportPlayer;
                        win.SetActive(false);
                        lose.SetActive(false);
                        restartLevel.SetActive(false);
                        nextLevel.SetActive(true);
                        inTransition = true;
                        AudioClip audio3 = level3Instr.clip;
                        if (audio3 != null && !level3Instr.isPlaying)
                        {
                            level1Instr.PlayOneShot(audio3);
                        }
                        Debug.Log("player has won level! Loading next level");
                        internalActive = false;
                        externalActive = false;
                        levelEnded = false;
                        levelEndedObject.SetActive(false);
                        LoadNextLevel("ClutterRoom", "AudioRoom");
                    }
                    else
                    {
                        status = GameStatus.LOST;
                        keysHeld--;
                        inTransition = true;
                        player.transform.position = transportPlayer;
                        // check how many lives player has left, if 0, transport to "lose" room
                        if (keysHeld <= 0)
                        {
                            // turn off "transition" room and turn on "lose" room
                            Debug.Log("player has lost game!");
                            win.SetActive(false);
                            lose.SetActive(true);
                            restartLevel.SetActive(false);
                            nextLevel.SetActive(false);
                        }
                        else
                        {
                            // turn on "transition" room
                            win.SetActive(false);
                            lose.SetActive(false);
                            restartLevel.SetActive(true);
                            nextLevel.SetActive(false);
                            AudioClip audio2 = level2Instr.clip;
                            if (audio2 != null && !level2Instr.isPlaying)
                            {
                                level2Instr.PlayOneShot(audio2);
                            }
                            Debug.Log("player has lost level! Reloading level");
                            internalActive = false;
                            externalActive = false;
                            numKeys = 0;
                            levelEnded = false;
                            levelEndedObject.SetActive(false);
                            ReloadLevel("ClutterRoom");
                        }

                    }
                }
            }
        }
        else if (currLevel == Level.LEVELTHREE)
        {
            if (inTransition && obj.tag == "Repeat")
            {
                // repeat the instructions for level 3
                if (!hasWon)
                {
                    AudioClip audio3 = level3Instr.clip;
                    if (audio3 != null && !level3Instr.isPlaying)
                    {
                        level3Instr.PlayOneShot(audio3);
                    }
                }
            }
            // check if player is ready to go to the next level 
            else if (inTransition && obj.tag == "Ready")
            {
                isReady = true;
            }
            else if (levelEnded)
            {
                // check if player collided with door, move to transition room
                if (h == "LeftHand" && obj.gameObject.tag == "Door")
                {
                    if (hasWon)
                    {
                        status = GameStatus.WON;
                        // turn off "transition" room and turn on "win" room
                        win.SetActive(true);
                        lose.SetActive(false);
                        restartLevel.SetActive(false);
                        nextLevel.SetActive(false);
                        player.transform.position = transportPlayer;
                        inTransition = true;
                        internalActive = false;
                        externalActive = false;
                        levelEndedObject.SetActive(false);
                    }
                    else
                    {
                        status = GameStatus.LOST;
                        keysHeld--;
                        inTransition = true;
                        player.transform.position = transportPlayer;
                        // check how many lives player has left, if 0, transport to "lose" room
                        if (keysHeld <= 0)
                        {
                            // turn off "transition" room and turn on "lose" room
                            win.SetActive(false);
                            lose.SetActive(true);
                            restartLevel.SetActive(false);
                            nextLevel.SetActive(false);
                            Debug.Log("player has lost game!");
                        }
                        else
                        {
                            // turn on transition room
                            win.SetActive(false);
                            lose.SetActive(false);
                            restartLevel.SetActive(true);
                            nextLevel.SetActive(false);
                            AudioClip audio3 = level3Instr.clip;
                            if (audio3 != null && !level3Instr.isPlaying)
                            {
                                level3Instr.PlayOneShot(audio3);
                            }
                            Debug.Log("player has lost level! Reloading level");
                            internalActive = false;
                            externalActive = false;
                            levelEndedObject.SetActive(false);
                            ReloadLevel("AudioRoom");
                        }
                    }
                }
            }
        }
    }





    void updateLevelOne()
    {
        if (status == GameStatus.RUNNING)
        {
            Debug.Log(startTime);
            // make sure object references are set
            if (room == null)
                room = GameObject.Find("Room");
            if (visibleDoor == null)
            {
                visibleDoor = GameObject.FindWithTag("Door");
                targetDoor = GameObject.Find("Target Position");
            }

            // hide the door after specified time
            if ((Time.time - startTime) > doorVisibleTime)
            {
                visibleDoor.SetActive(false);
            }

            // get reference to all the keys in the room
            if (!findKeys)
            {
                keys = GameObject.FindGameObjectsWithTag("Key");
                totalKeys = keys.Length;
                Debug.Log("total keys in scene is: " + totalKeys);
                for (int i = 0; i < totalKeys; i++)
                {
                    keys[i].SetActive(false);
                }
                findKeys = true;
            }

            // if there are still keys in the room
            if (!keyEnabled && keysHeld != 5)
            {
                Debug.Log("enabling key");
                int randomKey = Random.Range(0, totalKeys);
                if (keys[randomKey] != null)
                {
                    currentEnabledKey = keys[randomKey];
                    Debug.Log("enabled key is: " + randomKey);
                    Debug.Log(currentEnabledKey.name);
                    currentEnabledKey.SetActive(true);
                    keyEnabled = true;
                }   
                else
                {
                    randomKey = Random.Range(1, totalKeys);
                }
            }

            // demon controls (external player)
            if ((Time.time - startTime) < levelOnePlayerDuration)
            {
                // make sure only external player can play
                if (externalActive == false)
                {
                    popUpStart = Time.time;
                    Debug.Log("external player time");
                    internalActive = false;
                    externalActive = true;
                }

                // popup expires
                if ((Time.time - popUpStart) >= popUpDuration)
                {
                    externalPlayerPopUp.SetActive(false);
                }

            }
            // player controls (internal player)
            else if ((Time.time - startTime) >= levelOnePlayerDuration && (Time.time - startTime) < levelOneTotalDuration)
            {
                // first time internal player is activated
                if (internalActive == false)
                {
                    Debug.Log("internal player time");
                    internalActive = true;
                    externalActive = false;
                    internalPlayerPopUp.SetActive(true);
                    externalPlayerPopUp.SetActive(false);

                    popUpStart = Time.time;
                }

                // pop up expires
                if ((Time.time - popUpStart) >= popUpDuration)
                {
                    internalPlayerPopUp.SetActive(false);
                }

            }
            // if time exceeds total playing time of the level
            else if ((Time.time - startTime) >= levelOneTotalDuration)
            {
                levelEnded = true;
                levelEndedObject.SetActive(true);
                Debug.Log("level ended, please interact with door");
                // show door after level ends
                visibleDoor.SetActive(true);
                internalActive = false;
                externalActive = false;
                float totalDoorDistance = Vector3.Distance(visibleDoor.transform.position, targetDoor.transform.position);
                if (totalDoorDistance <= thresholdDistance)  //check if the two doors are less than x meters apart
                {
                    Debug.Log("player has beat level one!");
                    hasWon = true;
                    inTransition = true;
                }
                else
                {
                    Debug.Log("player has lost!");
                    hasWon = false;
                    inTransition = true;
                }
            }
        }
    }

    void updateLevelTwo()
    {
        if (status == GameStatus.RUNNING)
        {         
            // demon controls (external player)
            if ((Time.time - startTime) < levelTwoPlayerDuration)
            {
                if (externalActive == false)
                {
                    popUpStart = Time.time;
                    Debug.Log("external player time");
                    internalActive = false;
                    externalActive = true;
                }

                // popup expires
                if ((Time.time - popUpStart) >= popUpDuration)
                {
                    externalPlayerPopUp.SetActive(false);
                }

            }
            // player controls (internal player)
            else if ((Time.time - startTime) >= levelTwoPlayerDuration && (Time.time - startTime) < levelTwoTotalDuration)
            {
                // first time internal player is activated
                if (internalActive == false)
                {
                    Debug.Log("internal player time");
                    internalActive = true;
                    externalActive = false;

                    internalPlayerPopUp.SetActive(true);
                    externalPlayerPopUp.SetActive(false);

                    popUpStart = Time.time;
                }

                // pop up expires
                if ((Time.time - popUpStart) >= popUpDuration)
                {
                    internalPlayerPopUp.SetActive(false);
                }
            }
            // if time exceeds total playing time of the level
            else if ((Time.time - startTime) >= levelTwoTotalDuration)
            {
                levelEnded = true;
                levelEndedObject.SetActive(true);
                Debug.Log("level ended, please interact with door");
                internalActive = false;
                externalActive = false;
                //check if all keys have been collected
                if (numKeys >= 3)
                {
                    Debug.Log("player has beat level!");
                    hasWon = true;
                    inTransition = true;
                }
                else
                {
                    Debug.Log("player has lost level!");
                    hasWon = false;
                    inTransition = true;
                }
            }
        }
    }
    

    void updateLevelThree()
    {
        if (status == GameStatus.RUNNING)
        {
            if (!internalActive && !externalActive)
            {
                internalActive = true;
                externalActive = true;
            }
            // game playing
            if ((Time.time - startTime) < levelThreeTotalDuration)
            {
                // check that the music object has been instantiated
                if (musicActive && audioObject != null)
                {
                    // calculate Vector from audio object and right controller (external player)
                    Vector3 controllerPos = rightHand.transform.position;
                    Vector3 soundPos = audioObject.transform.position;

                    // normalize vector so length of 1
                    Vector3 distVector = (controllerPos - soundPos).normalized;

                    // add vector to sound position each frame to follow audio object
                    audioObject.transform.position += distVector * Time.deltaTime * followSpeed;
                }
            }
            // player loses
            else
            {
                levelEnded = true;
                levelEndedObject.SetActive(true);
                Debug.Log("level ended, please interact with door");
                internalActive = false;
                externalActive = false;

                if (musicCollected >= 3) 
                {
                    Debug.Log("player has beat level three!");
                    hasWon = true;
                    inTransition = true;
                }
                else
                {
                    Debug.Log("player has lost level three!");
                    hasWon = false;
                    inTransition = true;
                }
            }
        }
    }



    /**
     * Press the trigger button on the controller
     */
    public void ButtonTrigger(float value, string h)
    {
        hand = h;
        Debug.Log(hand);
        switch (currLevel)
        {
            case Level.LEVELONE:
                //if (value > 0.1f)
                //{
                    // internal player trying to pick up key 
                    //if (hand == "left")
                    //{
                    //    // check if internal player clicked on audioObject
                    //    if (keyEnabled)
                    //    {
                    //        float distFromObj = Vector3.Distance(leftHand.transform.position, currentEnabledKey.transform.position);
                    //        if (distFromObj <= controllerThreshold)
                    //        {
                    //            Debug.Log("TOUCHED KEY");
                    //            keysHeld++;
                    //            if (currentEnabledKey != null)
                    //            {
                    //                Destroy(currentEnabledKey);
                    //            }
                    //            keyEnabled = false; 
                    //        }
                    //    }
                    //}
                //}
                break;
            case Level.LEVELTWO:
                selectedButton = ui.buttonWasSelected();
                if (value > 0.1f)
                {
                    // internal player trying to pick up object 
                    //if (hand == "left" && internalActive)
                    //{
                    //    float distFromSound = Vector3.Distance(leftHand.transform.position, audioObject.transform.position);
                    //    if (distFromSound <= soundDist)
                    //    {
                    //        musicCollected += 1;
                    //        Destroy(audioObject);
                    //        musicActive = false;
                    //    }
                    //}                   
                }
                if (hand == "right" && selectedButton)
                {
                    Debug.Log(selectedButton);
                    if ((value > 0.7f) && !pressed)
                    {
                        OnPressed();
                        pressed = true;
                    }

                    if ((value < 0.2f) && pressed)
                    {
                        OnRelease();
                        pressed = false;
                    }
                }
                
                break;
            case Level.LEVELTHREE:
                selectedButton = ui.buttonWasSelected();
                if (value > 0.1f)
                {
                    // spawn new sound if no active music is playing
                    if (hand == "right" && !musicActive && externalActive)
                    {
                        Debug.Log("trying to spawn new sound");
                        // spawn in new sound
                        OnPressed();
                    }

                    // internal player trying to select where they think sound is 
                    if (hand == "left" && internalActive)
                    {
                        // check if internal player clicked on audioObject
                        if (musicActive && audioObject != null)
                        {
                            float distFromSound = Vector3.Distance(leftHand.transform.position, audioObject.transform.position);
                            if (distFromSound <= soundDist)
                            {
                                musicCollected += 1;
                                Destroy(audioObject);
                                musicActive = false;
                            }
                        }
                    }
                }
                    break;
            default:
                break;
        }
    }

    public void ButtonKeypadPress(float value, string h)
    {

        switch (currLevel)
        {
            case Level.LEVELZERO:
                ui.SetLevelMenu(0);
                break;
            case Level.LEVELONE:
                ui.SetLevelMenu(1);
                break;
            case Level.LEVELTWO:
                ui.SetLevelMenu(2);
                break;
            case Level.LEVELTHREE:
                ui.SetLevelMenu(3);
                break;
            default:               
                break;
        }
    }

    /**
     * Touch the keypad on the controller.
     */
    public void ButtonKeypadTouch(Vector2 value, string h)
    {
        hand = h;

        // only get x value
        float x_val = value.x;
        switch (currLevel)
        {
            case Level.LEVELONE:
                if (Mathf.Abs(x_val) > 0.1f)
                {
                    if (hand == "left" && internalActive)
                    {
                        room.transform.Rotate(new Vector3(0, x_val * rotationSpeed, 0) * Time.deltaTime);
                        Debug.Log(x_val);
                    }
                    else if (hand == "right" && externalActive)
                    {
                        room.transform.Rotate(new Vector3(0, -(x_val * rotationSpeed), 0) * Time.deltaTime);
                        Debug.Log(x_val);
                    }
                }
                break;
            case Level.LEVELTWO:
                
                break;
            case Level.LEVELTHREE:
                break;
            default:
                break;
        }
    }



    public void OnPressed()
    {
        Debug.Log("external player pressed trigger");

        // should only be called by external player 2 (right hand)
        if (externalActive)
        {
            if (currLevel == Level.LEVELTWO)
            {
                // instantiate a new object where the right controller is
                GameObject clone = ui.getCurrentSelectedObject();
                if (clone != null)
                {
                    Debug.Log("instantiating object: " + clone.name);
                    recentCube = Instantiate(clone, rightHand.transform.position, rightHand.transform.rotation);
                    recentCube.SetActive(true);
                    Rigidbody rigidbody = recentCube.GetComponent<Rigidbody>();
                    rigidbody.isKinematic = true;
                }
                //recentCube = Instantiate(spawnObject, rightHand.transform.position, rightHand.transform.rotation);
                objectsPlaced.Add(recentCube);
            }            
        }
        if (currLevel == Level.LEVELTHREE)
        {
            // spawn new audio object
            GameObject clone = ui.getCurrentSelectedObject();
            if (clone != null)
            {
                Debug.Log("instantiating sound: " + clone.name);
                audioObject = Instantiate(clone, rightHand.transform.position, rightHand.transform.rotation);
                audioSource = audioObject.GetComponent<AudioSource>();
                audioObject.SetActive(true);
                musicActive = true;
            }
        }
    }

    void OnRelease()
    {
        // should only be called by external player 2 (right hand)
        if (externalActive)
        {
            Rigidbody rigidbody = recentCube.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
        }
    }

    public int getLevel()
    {
        if (currLevel == Level.LEVELZERO)
            return 0;
        else if (currLevel == Level.LEVELONE)
            return 1;
        else if (currLevel == Level.LEVELTWO)
            return 2;
        else
            return 3;
    }

    void LoadNextLevel(string unloadScene, string loadScene)
    {
        SceneManager.UnloadSceneAsync(unloadScene);
        SceneManager.LoadScene(loadScene, LoadSceneMode.Additive);
    }

    void ReloadLevel(string loadScene)
    {
        SceneManager.UnloadSceneAsync(loadScene);
        SceneManager.LoadScene(loadScene, LoadSceneMode.Additive);
    }
}