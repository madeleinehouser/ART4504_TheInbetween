using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSound : MonoBehaviour
{
    // Start is called before the first frame update

    public AudioSource audio;
    

    void Start()
    {
     
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown("space") && !audio.isPlaying)
                 audio.Play();
       
 
        
    }
}
