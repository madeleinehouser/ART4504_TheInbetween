using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public GameObject levelMenu2;
    public GameObject levelMenu3;
    public int linewidth = 7;
    int currentPosX2 = 0;       // cannot exceed 0 and 3
    int currentPosX = 0;        // cannot exceed 0 and 7
    int currentPosY = 0;        // cannot exceed 0 and 1

    int currentSelected;
    int currLevel;
    GameObject currSelectedButton;
    GameObject currObjSelected;

    GameObject mainGameObj;
    Main main;
    int level;
    GameObject levelMenu;
    bool selected;

    // Start is called before the first frame update
    void Start()
    {
        selected = false;
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
        // check X & Y values, whichever value is greater, move that way if possible
        float x = value.x;
        float y = value.y;
        if (currLevel == 2)
        {
            selected = true;
            if (currObjSelected == null)
            {
                EventSystem.current.SetSelectedGameObject(levelMenu2.transform.GetChild(0).gameObject);
                currSelectedButton = EventSystem.current.currentSelectedGameObject.gameObject;
                currObjSelected = currSelectedButton.transform.GetChild(0).gameObject;
            }

            // check direction
            if (Mathf.Abs(x) > Mathf.Abs(y))    // move either right or left
            {
                if (x < 0)  // move left
                {
                    // make sure we can move left
                    if (currentPosX2 > 0 && currentPosX2 <= 3)
                    {
                        currentPosX2 -= 1;
                        currentSelected -= 1;
                        EventSystem.current.SetSelectedGameObject(levelMenu2.transform.GetChild(currentSelected).gameObject);
                        currSelectedButton = EventSystem.current.currentSelectedGameObject.gameObject;
                        currObjSelected = currSelectedButton.transform.GetChild(0).gameObject;
                    }
                }
                else // move right
                {
                    if (currentPosX2 >= 0 && currentPosX2 < 3)
                    {
                        // make sure we can move right
                        currentPosX2 += 1;
                        currentSelected += 1;
                        EventSystem.current.SetSelectedGameObject(levelMenu2.transform.GetChild(currentSelected).gameObject);
                        currSelectedButton = EventSystem.current.currentSelectedGameObject.gameObject;
                        currObjSelected = currSelectedButton.transform.GetChild(0).gameObject;
                    }
                }
            }
            //else    // move up or down
            //{
            //    if (y > 0)  // move up
            //    {
            //        if (currentPosY == 1)   // make sure we can move up
            //        {
            //            currentPosY = 0;
            //            //currObjSelected.SetActive(false);
            //            currentSelected += linewidth;
            //            EventSystem.current.SetSelectedGameObject(levelMenu2.transform.GetChild(currentSelected).gameObject);
            //            currObjSelected = EventSystem.current.currentSelectedGameObject.gameObject;
            //            selectButton = currObjSelected.transform.GetChild(1).gameObject;
            //            //selectButton.SetActive(true);

            //        }
            //    }
            //    else // move down
            //    {
            //        if (currentPosY == 0) // make sure we can move down
            //        {
            //            currentPosY = 1;
            //            //currObjSelected.SetActive(false);
            //            currentSelected += linewidth;
            //            EventSystem.current.SetSelectedGameObject(levelMenu2.transform.GetChild(currentSelected).gameObject);
            //            currObjSelected = EventSystem.current.currentSelectedGameObject.gameObject;
            //            selectButton = currObjSelected.transform.GetChild(1).gameObject;
            //            //selectButton.SetActive(true);
            //        }
            //    }
            //}
        }
        else if (currLevel == 3)
        {
            selected = true;
            Debug.Log(currLevel);
            if (currObjSelected == null)
            {
                Debug.Log(levelMenu3.transform.GetChild(0).name);
                EventSystem.current.SetSelectedGameObject(levelMenu3.transform.GetChild(0).gameObject);
                Debug.Log(EventSystem.current.currentSelectedGameObject.name);
                currSelectedButton = EventSystem.current.currentSelectedGameObject.gameObject;
                currObjSelected = currSelectedButton.transform.GetChild(0).gameObject;
                Debug.Log(currObjSelected.name);
            }

            // check direction
            if (Mathf.Abs(x) > Mathf.Abs(y))    // move either right or left
            {                    
                if (x < 0)  // move left
                {
                    Debug.Log("want to move left or right");
                    if (currentPosX > 0 && currentPosX <= linewidth)
                    {
                        Debug.Log("PRESSED LEFT");
                        // make sure we can move left
                        currentPosX -= 1;
                        currentSelected -= 1;
                        Debug.Log(currentSelected);
                        EventSystem.current.SetSelectedGameObject(levelMenu3.transform.GetChild(currentSelected).gameObject);
                        currSelectedButton = EventSystem.current.currentSelectedGameObject.gameObject;
                        currObjSelected = currSelectedButton.transform.GetChild(0).gameObject;
                    }
                }
                else // move right
                {
                    // make sure we can move right
                    if (currentPosX >= 0 && currentPosX < linewidth)
                    {
                        Debug.Log("PRESSED RIGHT");
                        currentPosX += 1;
                        currentSelected += 1;
                        Debug.Log(currentSelected);
                        EventSystem.current.SetSelectedGameObject(levelMenu3.transform.GetChild(currentSelected).gameObject);
                        currSelectedButton = EventSystem.current.currentSelectedGameObject.gameObject;
                        currObjSelected = currSelectedButton.transform.GetChild(0).gameObject;
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
                        currentSelected -= linewidth;
                        EventSystem.current.SetSelectedGameObject(levelMenu3.transform.GetChild(currentSelected).gameObject);
                        currSelectedButton = EventSystem.current.currentSelectedGameObject.gameObject;
                        currObjSelected = currSelectedButton.transform.GetChild(0).gameObject;

                    }
                }
                else // move down
                {
                    if (currentPosY == 0) // make sure we can move down
                    {
                        Debug.Log("PRESSED DOWN");
                        currentPosY = 1;
                        currentSelected += linewidth;
                        EventSystem.current.SetSelectedGameObject(levelMenu3.transform.GetChild(currentSelected).gameObject);
                        currSelectedButton = EventSystem.current.currentSelectedGameObject.gameObject;
                        currObjSelected = currSelectedButton.transform.GetChild(0).gameObject;
                    }
                }
            }
        }
    }

    public GameObject getCurrentSelectedObject()
    {
        return currObjSelected;
    }

    public void SetLevelMenu(int level)
    {
        currLevel = level;
        if (level == 2)
        {
            levelMenu2.SetActive(true);
            levelMenu3.SetActive(false);
        }
        else if (level == 3)
        {
            levelMenu2.SetActive(false);
            levelMenu3.SetActive(true);
        }
        else return;
    }

    public bool buttonWasSelected()
    {
        return selected;
    }
}
