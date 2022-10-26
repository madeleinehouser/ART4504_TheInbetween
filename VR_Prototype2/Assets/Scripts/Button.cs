using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.XR.OpenXR.Input;

public class Button : MonoBehaviour
{
    public InputActionReference interactAction;
    bool pressed;
    public GameObject rHand;
    public GameObject spawnObject; //object to spawn

    private GameObject recentCube;

    // Start is called before the first frame update
    void Start()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteractAction;
        pressed = false;

    }

    // Update is called once per frame
    void Update()
    {
        
        
    }


    void OnPressed()
    {
        recentCube = Instantiate(spawnObject,rHand.transform.position, rHand.transform.rotation);
        //newobject enable the physics
        
        Rigidbody rigidbody = recentCube.GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;

    }


    void OnRelease()
    {
        Rigidbody rigidbody = recentCube.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;

    }

    protected void OnInteractAction(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();

        if ((value > 0.8f) && !pressed)
        {
            OnPressed();
            pressed = true;
        }

        if ((value < 0.2f) && pressed)
        {
            OnRelease();
            pressed = false;
        }

        
        Debug.Log("status" + pressed);
    }
}
