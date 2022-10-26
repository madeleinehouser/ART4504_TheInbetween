using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    enum Level { LEVELONE, LEVELTWO, LEVELTHREE };
    enum GameStatus { RUNNING, WON, LOST };

    public Camera player;
    int level_counter;
    public GameObject transitionCube;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject lose;
    public GameObject win;
    Level currLevel;
    GameStatus status;
    string hand;
    // left hand - internal
    bool internalActive;
    // right hand - external
    bool externalActive;

    bool hasLoaded;

    // timer for each level
    float startTime;            //level start time - resets every time a new level loads
    float transitionTime;             //transition start time - resets every time the game ends (duration ends)
    public float demonDuration;
    public float playerDuration;
    public float totalDuration;        //duration of running state
    float loadingDuration; //duration of transitions (GameWon or GameLost states)
    
    // level 1 vars
    public float rotationSpeed;
    GameObject room;
    public GameObject visibleDoor;
    GameObject targetDoor;
    public float thresholdDistance;     // distance door has to be from target position to be considered "winning"
    float doorVisibleTime;              // total time door is visible when level 1 starts before disappearing
    public bool hasWon;
    public GameObject internalPlayerTime;   // pop up showing internal player time to play
    public GameObject externalPlayerTime;   // pop up showing external player time to play
    float popUpStart;       // time pop up starts
    float popUpDuration;    // total time pop up should be visible
    GameObject[] keys;
    int totalKeys;
    bool keyEnabled;
    int keysHeld;
    bool findKeys;

    // level 2 vars
    public int numKeys;
    public GameObject spawnObject;
    private GameObject recentCube;
    bool pressed;
    bool gameEnded;

    // level 3 vars
    public GameObject audioObject;
    public AudioSource audioSource;
    public GameObject spawnAudio;
    bool musicActive;

    //DetectCollisions col;

    // Start is called before the first frame update
    void Start()
    {
        level_counter = 0;
        leftHand = GameObject.Find("LeftHand");
        rightHand = GameObject.Find("RightHand");
        transitionCube.SetActive(false);
        internalActive = false;
        externalActive = false;
        hasWon = false;
        hasLoaded = false;

        SceneManager.LoadScene("SpinningRoom", LoadSceneMode.Additive);
        totalDuration = playerDuration + demonDuration;
        popUpDuration = 3f;
        loadingDuration = 5f;
        doorVisibleTime = 5f;

        //set everything to start level one
        startTime = Time.time;
        currLevel = Level.LEVELONE;
        status = GameStatus.RUNNING;
        keyEnabled = false;
        keysHeld = 0;
        totalKeys = 0;
        findKeys = false;
 
        rotationSpeed = 5f;      // set level 1 vars
        thresholdDistance = 1f;
        internalPlayerTime.SetActive(false);
        externalPlayerTime.SetActive(true);
        //--------------------

        keys = new GameObject[5];

        // set level 2 vars
        numKeys = 0;
        pressed = false;
        gameEnded = false;

        //win.GetComponent<Canvas>().worldCamera = player;
        //lose.GetComponent<Canvas>().worldCamera = player;

        // set level 3 vars
        musicActive = false;
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
                            Debug.Log("transition time ended");
                            transitionCube.SetActive(false);
                            //win.SetActive(false);
                            currLevel = Level.LEVELTWO;
                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            internalActive = true;
                            externalActive = true;
                        }
                        break;
                    case GameStatus.LOST:
                        if ((Time.time - transitionTime) > loadingDuration)
                        {
                            transitionCube.SetActive(false);
                            Debug.Log("lost duration time ENDED");
                            //lose.SetActive(false);
                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            internalActive = true;
                            externalActive = true;
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
                            transitionCube.SetActive(false);
                            //win.SetActive(false);
                            startTime = Time.time;
                            currLevel = Level.LEVELTHREE;
                            status = GameStatus.RUNNING;
                            internalActive = true;
                            externalActive = true;
                        }

                        break;
                    case GameStatus.LOST:                        
                        if ((Time.time - transitionTime) > loadingDuration)
                        {
                            transitionCube.SetActive(false);
                            //lose.SetActive(false);
                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            internalActive = true;
                            externalActive = true;
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
                            transitionCube.SetActive(false);
                            //win.SetActive(false);
                            startTime = Time.time;
                            currLevel = Level.LEVELTHREE;
                            status = GameStatus.RUNNING;

                            startTime = Time.time;
                            internalActive = true;
                            externalActive = true;
                        }
                        break;
                    case GameStatus.LOST:
                        if ((Time.time - transitionTime) > loadingDuration)
                        {
                            transitionCube.SetActive(false);
                            Debug.Log("lost duration time ENDED");
                            //lose.SetActive(false);
                            status = GameStatus.RUNNING;
                            startTime = Time.time;
                            internalActive = true;
                            externalActive = true;
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

                    if (h == "LeftHand" && obj.gameObject.tag == "Door")
                    {
                        if (hasWon)
                        {
                            status = GameStatus.WON;
                            transitionTime = Time.time;
                            LoadNextLevel("SpinningRoom", "Clutter Room");
                            transitionCube.SetActive(true);
                            //win.SetActive(true);
                            internalActive = false;
                            externalActive = false;
                        }
                        else
                        {
                            status = GameStatus.LOST;
                            transitionTime = Time.time;
                            //lose.SetActive(true);
                            transitionCube.SetActive(true);
                            ReloadLevel("SpinningRoom");
                            internalActive = false;
                            externalActive = false;
                        }
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

                    // only when game ends, check win condition
                    if (gameEnded)
                    {
                        Debug.Log("go into door");
                        // if interact with door, 
                        if (h == "LeftHand" && obj.gameObject.tag == "Door")
                        {
                            Debug.Log("hand collided with door");
                            if (hasWon)
                            {
                                status = GameStatus.WON;
                                transitionTime = Time.time;
                                transitionCube.SetActive(true);
                                gameEnded = false;
                                //win.SetActive(true);
                                LoadNextLevel("Clutter Room", "AudioRoom_Player1");
                                internalActive = false;
                                externalActive = false;
                            }
                            else
                            {
                                status = GameStatus.LOST;
                                //lose.SetActive(true);
                                transitionCube.SetActive(true);
                                numKeys = 0;
                                gameEnded = false;
                                ReloadLevel("Clutter Room");
                                transitionTime = Time.time;
                                internalActive = false;
                                externalActive = false;
                            }
                        }
                    }
                    break;
                case Level.LEVELTHREE:
                    {
                        if (hasWon)
                        {
                            status = GameStatus.WON;
                            transitionTime = Time.time;
                            transitionCube.SetActive(true);
                            //win.SetActive(true);
                            internalActive = false;
                            externalActive = false;
                        }
                        else
                        {
                            status = GameStatus.LOST;
                            //lose.SetActive(true);
                            transitionCube.SetActive(true);
                            transitionTime = Time.time;
                            internalActive = false;
                            externalActive = false;
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
                    Debug.Log("disabled key " + i);

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
            if ((Time.time - startTime) < playerDuration)
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
            else if ((Time.time - startTime) >= playerDuration && (Time.time - startTime) < totalDuration)
            {
                // first time internal player is activated
                if (internalActive == false)
                {
                    Debug.Log("internal player ime");
                    internalActive = true;
                    externalActive = false;
                    internalPlayerTime.SetActive(true);

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
            else if ((Time.time - startTime) >= totalDuration)
            {
                Debug.Log("level ended, please interact with door");
                // show door after level ends
                visibleDoor.SetActive(true);

                float totalDoorDistance = Vector3.Distance(visibleDoor.transform.position, targetDoor.transform.position);
                if (totalDoorDistance <= thresholdDistance)  ///check if the two doors are less than x meters apart
                {
                    hasWon = true;
                }
                else
                {
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
            if ((Time.time - startTime) < playerDuration)
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
            else if ((Time.time - startTime) >= playerDuration && (Time.time - startTime) < totalDuration)
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
            else if ((Time.time - startTime) >= totalDuration)
            {
                gameEnded = true;
                Debug.Log("level ended, please interact with door");

                if (numKeys >= 3)  //check if all keys have been collected
                {
                    hasWon = true;
                }
                else
                {
                    hasWon = false;
                }
            }
        }
    }
    

    void updateLevelThree()
    {
        //rightHand.tag = "Audio";
        if (status == GameStatus.RUNNING)
        {
            if (audioObject == null)
            {
                audioObject = GameObject.Find("AudioObject1");
                if (audioObject == null)
                    Debug.Log("cannot found audio object");
                else
                    audioSource = audioObject.GetComponent<AudioSource>();

            }
            // player loses
            if ((Time.time - startTime) >= totalDuration)
            {
                status = GameStatus.LOST;
                //lose.SetActive(true);
                transitionCube.SetActive(true);
                ReloadLevel("AudioRoom_Player1"); //reload the existing 
                transitionTime = Time.time;
                internalActive = false;
                externalActive = false;
                //rightHand.SetActive(false);
                //leftHand.SetActive(false);
            }
        }
    }




    public void ButtonTrigger(float value, string h)
    {
        hand = h;
        Debug.Log("value for trigger: " + value);
        Debug.Log("hand is: " + hand);
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
                if (value > 0.1f)
                {
                    if (hand == "right" && externalActive)
                    {
                        if (!musicActive)
                        {
                            // spawn new audio object
                            audioObject = Instantiate(spawnAudio, rightHand.transform.position, rightHand.transform.rotation);
                            musicActive = true;
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



    void OnPressed()
    {
        // should only be called by external player 2 (right hand)
        if (externalActive && externalPlayerTime)
        {
            recentCube = Instantiate(spawnObject, rightHand.transform.position, rightHand.transform.rotation);
            //newobject enable the physics

            Rigidbody rigidbody = recentCube.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
        }
    }

    void OnRelease()
    {
        // should only be called by external player 2 (right hand)
        if (externalActive && externalPlayerTime)
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