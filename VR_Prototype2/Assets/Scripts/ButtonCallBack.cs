using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR.Input;

public class ButtonCallBack : MonoBehaviour
{
    public InputActionReference trigger;
    public InputActionReference keypadPress;
    public InputActionReference keypadTouch;

    GameObject mainGameObject;
    Main main;
    UIManager ui;

    Vector2 lastTouchedRight;
    Vector2 lastTouchedLeft;

    string hand;

    // Start is called before the first frame update
    void Start()
    {
        mainGameObject = GameObject.Find("Manager");
        main = mainGameObject.GetComponentInChildren<Main>();
        
        ui = GameObject.Find("ScreenUI").GetComponentInChildren<UIManager>();

        trigger.action.Enable();
        trigger.action.performed += OnButtonTrigger;

        keypadPress.action.Enable();
        keypadPress.action.performed += OnKeypadPress;

        keypadTouch.action.Enable();
        keypadTouch.action.performed += OnKeypadTouch;

        hand = "";
    }

    // Update is called once per frame
    void Update()
    {
           
    }


    protected void OnButtonTrigger(InputAction.CallbackContext ctx)
    {
      
        float v = ctx.ReadValue<float>();

        if (ctx.ReadValue<float>() > 0.1f)
        {
            InputAction act = ctx.action;
            string action = ctx.action.ToString();
            string[] actions = action.Split('/');
            if (actions[0] == "LeftHand")
            {
                hand = "left";
                //Debug.Log("called " + ctx.action);
                main.ButtonTrigger(ctx.ReadValue<float>(), hand);
            }
            // if right controller, rotate to the right
            else if (actions[0] == "RightHand")
            {
                hand = "right";
                //Debug.Log("called " + ctx.action);
                main.ButtonTrigger(ctx.ReadValue<float>(), hand);
            }

        }
    }

    protected void OnKeypadPress(InputAction.CallbackContext ctx)
    {
        float v = ctx.ReadValue<float>();

           InputAction act = ctx.action;
            string action = ctx.action.ToString();
            string[] actions = action.Split('/');
            if (actions[0] == "LeftHand")
            {
                hand = "left";
                //Debug.Log("called " + ctx.action);            
                ui.ButtonKeypadPress(lastTouchedLeft, hand);

            }
            // if right controller, rotate to the right
            else if (actions[0] == "RightHand")
            {
                hand = "right";
                //Debug.Log("called " + ctx.action);
                ui.ButtonKeypadPress(lastTouchedRight, hand);
            }
        
    }

    protected void OnKeypadTouch(InputAction.CallbackContext ctx)
    {
        Vector2 v = ctx.ReadValue<Vector2>();
       // Debug.Log(v);
        InputAction act = ctx.action;
       // Debug.Log(act);
        string action = ctx.action.ToString();
        string[] actions = action.Split('/');

        if (actions[0] == "LeftHand")
        {
            hand = "left";
          //  Debug.Log("called " + ctx.action);
            main.ButtonKeypadTouch(ctx.ReadValue<Vector2>(), hand);
            lastTouchedLeft = ctx.ReadValue<Vector2>();
        }
        // if right controller, rotate to the right
        else if (actions[0] == "RightHand")
        {
            hand = "right";
         //   Debug.Log("called " + ctx.action);
            main.ButtonKeypadTouch(ctx.ReadValue<Vector2>(), hand);
            lastTouchedRight = ctx.ReadValue<Vector2>();
        }
    }


    /*
     * Get which controller the trigger occurred on 
     * 
     * Returns a string of either "left" "right" or "" if no button has been pressed yet
     */
    public string getHand()
    {
        return hand;
    }

    float getActionValue(InputAction.CallbackContext c)
    {
        return c.ReadValue<float>();
    }
}
