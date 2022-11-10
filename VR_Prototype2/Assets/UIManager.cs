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

    GameObject currentSelected;
    int currLevel;

    // Start is called before the first frame update
    void Start()
    {
        currentSelected = null;
        levelMenu2.SetActive(false);
        levelMenu2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPress(Vector2 value, string h)
    {

        Debug.Log("press value:" + value);

        // check X & Y values, whichever value is greater, move that way if possible
        float x = value.x;
        float y = value.y;
        if (currLevel == 2)
        {
          
        }
        else if (currLevel == 3)
        {
            // have not yet made any selections
            if (currentSelected == null)
            {
                EventSystem.current.SetSelectedGameObject(levelMenu3.transform.GetChild(0).gameObject);
                currentSelected = EventSystem.current.currentSelectedGameObject;
            }
            else
            {
                // check direction
                if (Mathf.Abs(x) > Mathf.Abs(y))    // move either right or 
                {                    
                    if (x > 0)  // move left
                    {

                    }
                }
                else    // move up or down
                {
                    if (y > 0)  // move up
                    { 
                    }
                }
            }
        }
    }

    void SetLevelMenu(int level)
    {
        currLevel = level;
        if (level == 2)
            levelMenu2.SetActive(true);

        else
            levelMenu3.SetActive(false);

    }


}
