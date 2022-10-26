using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR.OpenXR.Input;

public class RotateRoom : MonoBehaviour
{
    public GameObject room;
    public float speed;

    float maxRotSpeed;

    GameObject leftHand;
    GameObject rightHand;

    // Start is called before the first frame update
    void Start()
    {
        maxRotSpeed = 10f;
    }

    // Update is called once per frame
    void Update()
    {       
     
    }

    public void rotateRoomLeft(float v)
    {
        room.transform.Rotate(new Vector3(0, v * maxRotSpeed, 0) * Time.deltaTime);
        Debug.Log("rotating left");
    }

    public void rotateRoomRight(float v)
    {
        room.transform.Rotate(new Vector3(0, -(v * maxRotSpeed), 0) * Time.deltaTime);
        Debug.Log("rotating right");   
    }
}
