using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandInteractions : MonoBehaviour
{
    Main main;
    GameObject mainGameObject;

    GameObject hand;
    string handName;

    // Start is called before the first frame update
    void Start()
    {
        handName = this.gameObject.name;
        hand = this.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        mainGameObject = GameObject.Find("Manager");
        main = mainGameObject.GetComponentInChildren<Main>();
        main.handCollided(handName, other.gameObject);
    }
}
