using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Main : MonoBehaviour
{
    enum Level { LEVELONE, LEVELTWO, LEVELTHREE };
    enum GameStatus { RUNNING, WON, LOST };

    public GameObject player;
    public GameObject transitionCube;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject lose;
    public GameObject win;
    Level currLevel;
    GameStatus status;
    string hand;
    bool internalActive;                    // controller for the internal player (left controller)
    bool externalActive;                    // controller for the external player (right controller)
    bool hasLoaded;                         // make sure the whole scene has loaded
    bool hasWon;                            // check if the internal player has won or lost
    Vector3 transportPlayer;

    // timer for each level
    float startTime;                        //level start time - resets every time a new level loads
    float transitionTime;                   //transition start time - resets every time the game ends (duration ends)
    float loadingDuration;                  //duration of transitions (GameWon or GameLost states)
    
    // level 1 vars
    public float rotationSpeed;             // speed to rotate the room for level 1
    GameObject room;                        // gameobject refers to the entire level 1 room (to rotate)
    public GameObject visibleDoor;          // gameobject refers to visible door in level 1
    GameObject targetDoor;                  // gameobject refers to original position of door in level 1
    public float thresholdDistance;         // distance door has to be from target position to be considered "winning"
    float doorVisibleTime;                  // total time door is visible when level 1 starts before disappearing
    public GameObject internalPlayerTime;   // pop up showing internal player time to play
    public GameObject externalPlayerTime;   // pop up showing external player time to play
    public GameObject levelEndedObject;     // pop up showing level has ended
    float popUpStart;                       // time pop up starts
    float popUpDuration;                    // total time pop up should be visible
    GameObject[] keys;                      // array that holds the keys to find in level 1
    int totalKeys;                          // total keys to be found in level 1
    bool keyEnabled;                        // boolean to tell if a key is active in level 1
    int keysHeld;                           // number of keys held (number of lives player has)
    bool findKeys;                          // bool that checks if the hidden keys in level 1 have been identified by Unity
    bool levelEnded;                         // check if the level has ended
    [SerializeField]
    private float levelOneTotalDuration;    // duration of running state for level 1
    public float levelOnePlayerDuration;    // duration of internal player for level 1
    public float levelOneDemonDuration;     // duration of external player for level 1

    // level 2 vars
    public int numKeys;                     // number of keys collected in level 2
    public GameObject spawnObject;          // hidden object for external player to spawn in level 2
    private GameObject recentCube;          // the most recent object that was spawned in level 2
    bool pressed;                           // check if the trigger button on the controller is being pressed
    [SerializeField]
    private float levelTwoTotalDuration;    // duration of running state for level 2
    public float levelTwoPlayerDuration;    // duration of internal player for level 2
    public float levelTwoDemonDuration;     // duration of external player for level 2

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
        leftHand = GameObject.Find("LeftHand");
        rightHand = GameObject.Find("RightHand");
        transitionCube.SetActive(false);
        internalActive = true;      // change to false later
        externalActive = true;      // change to false later;
        hasWon = true;
        hasLoaded = false;
        transportPlayer = new Vector3(-9.81000f, 2.94899f, 4.5982666f);

        SceneManager.LoadScene("AudioRoom_Player1", LoadSceneMode.Additive);
        levelOneTotalDuration = levelOnePlayerDuration + levelOneDemonDuration;
        popUpDuration = 3f;
        loadingDuration = 5f;
        doorVisibleTime = 5f;

        //set everything to start level one
        startTime = Time.time;
        currLevel = Level.LEVELTHREE;
        status = GameStatus.RUNNING;
        keyEnabled = false;
        keysHeld = 0;
        totalKeys = 0;
        findKeys = false;
 
        rotationSpeed = 5f;      // set level 1 vars
        thresholdDistance = 1f;
        internalPlayerTime.SetActive(false);
        externalPlayerTime.SetActive(true);
        levelEndedObject.SetActive(false);
        //--------------------

        keys = new GameObject[5];

        // set level 2 vars
        numKeys = 0;
        pressed = false;
        levelEnded = false;
        levelTwoTotalDuration = levelTwoPlayerDuration + levelTwoDemonDuration;

        // set level 3 vars
        musicActive = false;
        followSpeed = 0.2f;
        musicCollected = 0;
        soundDist = 0.2f;
    }




    // Update is called once per frame
    void Update()
    {
        if (!hasLoaded)
        {
            hasLoaded = true;
            return;
        }

        switch (currLevel)
        {
            case Level.LEVELONE:
                switch (status)
                {
                    case GameStatus.RUNNING:                        
                        updateLevelOne();
                        break;
                    case GameStatus.WON:
                        // load next level scene
                        if ((Time.time - transitionTime) > loadingDuration)
                        {
                            //transitionCube.SetActive(false);
                            player.transform.position = Vector3.zero;
                            currLevel = Level.LEVELTWO;
                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            internalActive = true;
                            externalActive = true;
                            levelEnded = false;
                            levelEndedObject.SetActive(false);
                        }
                        break;
                    case GameStatus.LOST:
                        if ((Time.time - transitionTime) > loadingDuration)
                        {
                            // lose a life
                            keysHeld -= 1;
                            //transitionCube.SetActive(false);
                            player.transform.position = Vector3.zero;

                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            internalActive = true;
                            externalActive = true;
                            levelEnded = false;
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
                        if ((Time.time - transitionTime) > loadingDuration)
                        {
                            //transitionCube.SetActive(false);
                            player.transform.position = Vector3.zero;

                            startTime = Time.time;
                            currLevel = Level.LEVELTHREE;
                            status = GameStatus.RUNNING;
                            internalActive = true;
                            externalActive = true;
                            levelEnded = false;
                            levelEndedObject.SetActive(false);

                        }

                        break;
                    case GameStatus.LOST:                        
                        if ((Time.time - transitionTime) > loadingDuration)
                        {
                            keysHeld -= 1;
                            //transitionCube.SetActive(false);
                            player.transform.position = Vector3.zero;

                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            internalActive = true;
                            externalActive = true;
                            levelEnded = false;
                            levelEndedObject.SetActive(false);
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
                        if ((Time.time - transitionTime) > loadingDuration)
                        {
                            //transitionCube.SetActive(false);
                            player.transform.position = Vector3.zero;

                            startTime = Time.time;
                            currLevel = Level.LEVELTHREE;
                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            internalActive = true;
                            externalActive = true;
                            levelEnded = false;
                            levelEndedObject.SetActive(false);
                        }
                        break;
                    case GameStatus.LOST:
                        if ((Time.time - transitionTime) > loadingDuration)
                        {
                            keysHeld -= 1;
                            //transitionCube.SetActive(false);
                            player.transform.position = Vector3.zero;

                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            internalActive = true;
                            externalActive = true;
                            levelEnded = false;
                            levelEndedObject.SetActive(false);
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


    public void handCollided(string h, GameObject obj)
    {
        if (internalActive)
        {
            switch (currLevel)
            {
                case Level.LEVELONE:
                    if (h == "LeftHand" && obj.gameObject.tag == "Key")
                    {
                        Debug.Log("TOUCHED KEY");
                        Destroy(obj);
                        keysHeld++;
                        keyEnabled = false;
                    }
                    break;

                case Level.LEVELTWO:
                    Debug.Log(h);
                    if (h == "LeftHand" && obj.gameObject.tag == "Key")
                    {
                        Debug.Log("TOUCHED KEY");
                        Destroy(obj);
                        numKeys++; 
                    }
                    break;

                case Level.LEVELTHREE:
                    //{
                        // only if level ended, check win condition
                        //if (levelEnded)
                        //{
                            
                        //}
                    //}
                    break;
                default:
                    break;
            }
        }
        else if (levelEnded)
        {
            switch (currLevel)
            {
                case Level.LEVELONE:
                    if (h == "LeftHand" && obj.gameObject.tag == "Door")
                    {
                        if (hasWon)
                        {
                            status = GameStatus.WON;
                            transitionTime = Time.time;
                            LoadNextLevel("SpinningRoom", "Clutter Room");
                            //transitionCube.SetActive(true);
                            // test transport of player
                            player.transform.position = transportPlayer;

                            internalActive = false;
                            externalActive = false;
                            levelEndedObject.SetActive(false);
                        }
                        else
                        {
                            status = GameStatus.LOST;
                            transitionTime = Time.time;
                            //transitionCube.SetActive(true);
                            // test transport of player
                            player.transform.position = transportPlayer;

                            ReloadLevel("SpinningRoom");
                            internalActive = false;
                            externalActive = false;
                            levelEndedObject.SetActive(false);
                        }
                    }
                    break;
                case Level.LEVELTWO:
                    Debug.Log("go into door");
                    // if interact with door, 
                    if (h == "LeftHand" && obj.gameObject.tag == "Door")
                    {
                        Debug.Log("hand collided with door");
                        if (hasWon)
                        {
                            status = GameStatus.WON;
                            transitionTime = Time.time;
                            //transitionCube.SetActive(true);
                            // test transport of player
                            player.transform.position = transportPlayer; //teleport to win room

                            levelEnded = false;
                            levelEndedObject.SetActive(false);
                            LoadNextLevel("Clutter Room", "AudioRoom_Player1");
                        }
                        else
                        {
                            status = GameStatus.LOST;
                            //transitionCube.SetActive(true);
                            player.transform.position = transportPlayer;
                            Debug.Log("tried to teleport");
                            numKeys = 0;
                            levelEnded = false;
                            levelEndedObject.SetActive(false);
                            ReloadLevel("Clutter Room");
                            transitionTime = Time.time;
                        }
                    }
                    break;
                case Level.LEVELTHREE:
                    if (h == "LeftHand" && obj.gameObject.tag == "Door")
                    {
                        if (hasWon)
                        {
                            status = GameStatus.WON;
                            transitionTime = Time.time;
                            //transitionCube.SetActive(true);
                            player.transform.position = transportPlayer;

                            levelEndedObject.SetActive(false);
                        }
                        else
                        {
                            status = GameStatus.LOST;
                            //transitionCube.SetActive(true);
                            player.transform.position = transportPlayer;

                            transitionTime = Time.time;
                            levelEndedObject.SetActive(false);
                            ReloadLevel("AudioRoom_Player1");
                        }
                    }
                    break;
                default:
                    break;
            }

        }
    }





    void updateLevelOne()
    {
        if (status == GameStatus.RUNNING)
        {
            // make sure object references are set
            if (room == null)
                room = GameObject.Find("Room");
            if (visibleDoor == null)
            {
                visibleDoor = GameObject.FindWithTag("Door");
                targetDoor = GameObject.Find("Target Position");
            }

            // hide the door after 5 seconds
            if (Time.time > doorVisibleTime)
            {
                visibleDoor.SetActive(false);
            }

            if (!findKeys)
            {
                keys = GameObject.FindGameObjectsWithTag("Key");
                totalKeys = keys.Length;
                Debug.Log("found total keys " + totalKeys);
                for (int i = 0; i < totalKeys; i++)
                {
                    //Debug.Log("disabled key " + i);
                    keys[i].SetActive(false);
                }
                findKeys = true;
            }

            if (!keyEnabled)
            {
                Debug.Log("enabling key");
                int randomKey = Random.Range(1, totalKeys);
                Debug.Log(randomKey);
                GameObject key = keys[randomKey];
                key.SetActive(true);
                keyEnabled = true;
            }

            // demon controls (external player)
            if ((Time.time - startTime) < levelOnePlayerDuration)
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
                    externalPlayerTime.SetActive(false);
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
                    internalPlayerTime.SetActive(true);
                    externalPlayerTime.SetActive(false);

                    popUpStart = Time.time;
                }

                // pop up expires
                if ((Time.time - popUpStart) >= popUpDuration)
                {
                    internalPlayerTime.SetActive(false);
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
                if (totalDoorDistance <= thresholdDistance)  ///check if the two doors are less than x meters apart
                {
                    Debug.Log("player has won!");
                    hasWon = true;
                }
                else
                {
                    Debug.Log("player has lost!");
                    hasWon = false;
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
                    externalPlayerTime.SetActive(false);
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

                    internalPlayerTime.SetActive(true);
                    externalPlayerTime.SetActive(false);

                    popUpStart = Time.time;
                }

                // pop up expires
                if ((Time.time - popUpStart) >= popUpDuration)
                {
                    internalPlayerTime.SetActive(false);
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
                    Debug.Log("player has won!");
                    hasWon = true;
                }
                else
                {
                    Debug.Log("player has lost!");
                    hasWon = false;
                }
            }
        }
    }
    

    void updateLevelThree()
    {
        if (status == GameStatus.RUNNING)
        {
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
                    Debug.Log("player has won!");
                    hasWon = true;
                }
                else
                {
                    Debug.Log("player has lost!");
                    hasWon = false;
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
        switch (currLevel)
        {
            case Level.LEVELONE:
                if (value > 0.2f)
                {
                    if (hand == "left" && internalActive)
                    {
                        room.transform.Rotate(new Vector3(0, value * rotationSpeed, 0) * Time.deltaTime);
                        Debug.Log("rotating left");
                    }
                    else if (hand == "right" && externalActive)
                    {
                        room.transform.Rotate(new Vector3(0, -(value * rotationSpeed), 0) * Time.deltaTime);
                        Debug.Log("rotating right");
                    }
                }
                break;
            case Level.LEVELTWO:
                if (hand == "right")
                {
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
                Debug.Log("l31");
                if (value > 0.1f)
                {
                    Debug.Log("l32");
                    // spawn new sound if no active music is playing
                    if (hand == "right" && !musicActive && externalActive)
                    {
                        Debug.Log("l33");
                        Debug.Log("trying to spawn new sound");
                        // spawn in new sound
                        OnPressed();
                        musicActive = true;
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

    public void ButtonTriggerPress(float value, string h)
    {

        switch (currLevel)
        {
            case Level.LEVELONE:
                
                break;
            case Level.LEVELTWO:
               
                break;
            case Level.LEVELTHREE:
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
        Debug.Log("instantiating sound");

       
        // should only be called by external player 2 (right hand)
        if (externalActive)
        {
            if (currLevel == Level.LEVELTWO)
            {
                // instantiate a new object where the right controller is
                recentCube = Instantiate(EventSystem.current.currentSelectedGameObject.transform.GetChild(0).gameObject, rightHand.transform.position, rightHand.transform.rotation);

                Rigidbody rigidbody = recentCube.GetComponent<Rigidbody>();
                rigidbody.isKinematic = true;
            }            
        }
        if (currLevel == Level.LEVELTHREE)
        {
            // spawn new audio object
            Debug.Log("instantin sou;" + EventSystem.current.currentSelectedGameObject.transform.GetChild(0).gameObject.name);
            audioObject = Instantiate(EventSystem.current.currentSelectedGameObject.transform.GetChild(0).gameObject, rightHand.transform.position, rightHand.transform.rotation);
            audioSource = audioObject.GetComponent<AudioSource>();
            
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