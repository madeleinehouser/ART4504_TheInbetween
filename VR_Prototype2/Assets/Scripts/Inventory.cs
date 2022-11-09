using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool panel;
    public GameObject inventoryPanel;

     // Start is called before the first frame update
     void Start() 
     {
        panel = true;
     }

     // Update is called once per frame
     void Update()
     {
        if (panel == true)
        {
            inventoryPanel.SetActive(true);
        }
        else
        {
            inventoryPanel.SetActive(false);
        }
     }
}
