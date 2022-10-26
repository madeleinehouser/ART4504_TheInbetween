using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR.Input;

public class ButtonCallbackAudio : MonoBehaviour
{

    public AudioSource audio;

    public InputActionReference interactAction;
    // Start is called before the first frame update
    void Start()
    {
        interactAction.action.Enable();
        interactAction.action.performed += OnInteractAction;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnInteractAction(InputAction.CallbackContext ctx)
    {



        if(!audio.isPlaying)
            audio.Play();
        else
            audio.Stop();
        
         Debug.Log("called" + ctx.ToString());
    }
}
