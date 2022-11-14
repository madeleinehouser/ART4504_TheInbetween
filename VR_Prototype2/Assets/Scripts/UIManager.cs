using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public GameObject levelMenu2;
    public GameObject levelMenu3;
    public int linewidth = 7;
    int currentPosX = 0;        // cannot exceed 0 and 7
    int currentPosY = 0;        // cannot exceed 0 and 1

    int currentSelected;
    int currLevel;
    GameObject currObjSelected;
    GameObject selectButton;

    GameObject mainGameObj;
    Main main;
    int level;
    GameObject levelMenu;

    // Start is called before the first frame update
    void Start()
    {
        mainGameObj = GameObject.Find("Manager");
        main = mainGameObj.GetComponentInChildren<Main>();
        levelMenu2.SetActive(false);
        levelMenu2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ButtonKeypadPress(Vector2 value, string h)
    {
        Debug.Log(levelMenu3.transform.GetChild(0).gameObject);
        Debug.Log("press value:" + value);

        // check X & Y values, whichever value is greater, move that way if possible
        float x = value.x;
        float y = value.y;
        if (currLevel == 2)
        {
            if (currObjSelected == null)
            {
                EventSystem.current.SetSelectedGameObject(levelMenu2.transform.GetChild(0).gameObject);
                currObjSelected = EventSystem.current.currentSelectedGameObject.gameObject;
                selectButton = currObjSelected.transform.GetChild(1).gameObject;
                selectButton.SetActive(true);
            }

            // check direction
            if (Mathf.Abs(x) > Mathf.Abs(y))    // move either right or left
            {
                if (x > 0)  // move left
                {
                    // make sure we can move left
                    if (currentPosX > 0 && currentPosX <= 7)
                    {
                        currentPosX -= 1;
                        currObjSelected.SetActive(false);
                        currentSelected -= 1;
                        EventSystem.current.SetSelectedGameObject(levelMenu2.transform.GetChild(currentSelected).gameObject);
                        currObjSelected = EventSystem.current.currentSelectedGameObject.gameObject;
                        selectButton = currObjSelected.transform.GetChild(0).gameObject;
                        selectButton.SetActive(true);
                    }
                }
                else // move right
                {
                    if (currentPosX >= 0 && currentPosX < 7)
                    {
                        // make sure we can move right
                        currentPosX += 1;
                        currObjSelected.SetActive(false);
                        currentSelected += 1;
                        EventSystem.current.SetSelectedGameObject(levelMenu2.transform.GetChild(currentSelected).gameObject);
                        currObjSelected = EventSystem.current.currentSelectedGameObject.gameObject;
                        selectButton = currObjSelected.transform.GetChild(0).gameObject;
                        selectButton.SetActive(true);
                    }
                }
            }
            else    // move up or down
            {
                if (y > 0)  // move up
                {
                    if (currentPosY == 1)   // make sure we can move up
                    {
                        currentPosY = 0;
                        currObjSelected.SetActive(false);
                        currentSelected += linewidth;
                        EventSystem.current.SetSelectedGameObject(levelMenu2.transform.GetChild(currentSelected).gameObject);
                        currObjSelected = EventSystem.current.currentSelectedGameObject.gameObject;
                        selectButton = currObjSelected.transform.GetChild(1).gameObject;
                        selectButton.SetActive(true);

                    }
                }
                else // move down
                {
                    if (currentPosY == 0) // make sure we can move down
                    {
                        currentPosY = 1;
                        currObjSelected.SetActive(false);
                        currentSelected += linewidth;
                        EventSystem.current.SetSelectedGameObject(levelMenu2.transform.GetChild(currentSelected).gameObject);
                        currObjSelected = EventSystem.current.currentSelectedGameObject.gameObject;
                        selectButton = currObjSelected.transform.GetChild(1).gameObject;
                        selectButton.SetActive(true);
                    }
                }
            }
        }
        else if (currLevel == 3)
        {
            if (currObjSelected == null)
            {
                EventSystem.current.SetSelectedGameObject(levelMenu3.transform.GetChild(0).gameObject);
                currObjSelected = EventSystem.current.currentSelectedGameObject.gameObject;
                selectButton = currObjSelected.transform.GetChild(0).gameObject;
                Debug.Log(selectButton);
                selectButton.SetActive(true);
            }

            // check direction
            if (Mathf.Abs(x) > Mathf.Abs(y))    // move either right or left
            {                    
                if (x < 0)  // move left
                {
                    Debug.Log("want to move left or right");
                    if (currentPosX > 0 && currentPosX <= 7)
                    {
                        Debug.Log("PRESSED LEFT");
                        // make sure we can move left
                        currentPosX -= 1;
                        //currObjSelected.SetActive(false);
                        currentSelected -= 1;
                        Debug.Log(currentSelected);
                        EventSystem.current.SetSelectedGameObject(levelMenu3.transform.GetChild(currentSelected).gameObject);
                        currObjSelected = EventSystem.current.currentSelectedGameObject.gameObject;
                        selectButton = currObjSelected.transform.GetChild(0).gameObject;
                        //selectButton.SetActive(true);
                    }
                }
                else // move right
                {
                    // make sure we can move right
                    if (currentPosX >= 0 && currentPosX < 7)
                    {
                        Debug.Log("PRESSED RIGHT");
                        currentPosX += 1;
                        //currObjSelected.SetActive(false);
                        currentSelected += 1;
                        Debug.Log(currentSelected);
                        EventSystem.current.SetSelectedGameObject(levelMenu3.transform.GetChild(currentSelected).gameObject);
                        currObjSelected = EventSystem.current.currentSelectedGameObject.gameObject;
                        selectButton = currObjSelected.transform.GetChild(1).gameObject;
                        //selectButton.SetActive(true);
                    }
                }
            }
            else    // move up or down
            {
                if (y > 0)  // move up
                {
                    if (currentPosY == 1)   // make sure we can move up
                    {
                        Debug.Log("PRESSED UP");
                        currentPosY = 0;                        
                        //currObjSelected.SetActive(false);
                        currentSelected -= linewidth;
                        EventSystem.current.SetSelectedGameObject(levelMenu3.transform.GetChild(currentSelected).gameObject);
                        currObjSelected = EventSystem.current.currentSelectedGameObject.gameObject;
                        selectButton = currObjSelected.transform.GetChild(1).gameObject;
                        //selectButton.SetActive(true);

                    }
                }
                else // move down
                {
                    if (currentPosY == 0) // make sure we can move down
                    {
                        Debug.Log("PRESSED DOWN");
                        currentPosY = 1;
                        //currObjSelected.SetActive(false);
                        currentSelected += linewidth;
                        EventSystem.current.SetSelectedGameObject(levelMenu3.transform.GetChild(currentSelected).gameObject);
                        currObjSelected = EventSystem.current.currentSelectedGameObject.gameObject;
                        selectButton = currObjSelected.transform.GetChild(1).gameObject;
                        //selectButton.SetActive(true);
                    }
                }
            }
        }
    }

    public GameObject getCurrentSelectedButton()
    {
        return currObjSelected;
    }

    public void SetLevelMenu(int level)
    {
        currLevel = level;
        if (level == 2)
        {
            //levelMenu = GameObject.Find("InventoryLevel2");
            levelMenu2.SetActive(true);
            levelMenu3.SetActive(false);
        }
        else if (level == 3)
        {
            //levelMenu = GameObject.Find("InventoryLevel3");
            levelMenu2.SetActive(false);
            levelMenu3.SetActive(true);
        }
    }


}
