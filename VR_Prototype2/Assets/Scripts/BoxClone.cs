using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxClone : MonoBehaviour
{
    public bool clone;
    // Start is called before the first frame update
    void Start()
    {
        clone = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isClone()
    {
        return clone;
    }
}
