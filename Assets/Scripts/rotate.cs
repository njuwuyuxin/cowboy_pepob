using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    
    private bool on_right = false;
    private bool on_left = false; 
    public float speed = 4f;
    public float angle = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
       

    }

    // Update is called once per frame
    void Update()
    {
        if(on_right)
        {
            if(transform.rotation.z>-angle&& transform.rotation.z < angle)
            {
                Debug.Log(transform.rotation.z);
                transform.Rotate(0, 0, -speed*Time.deltaTime);
                Debug.Log("right");
            }
           
        }
        else if(on_left)
        {
            if (transform.rotation.z < angle && transform.rotation.z > -angle)
            {
                transform.Rotate(0, 0, +speed*Time.deltaTime);
                Debug.Log("left");

            }
        }
    }
    public void call_right()
    {
        on_right = true;
        on_left = false;
    }
    public void call_left()
    {
        on_right = false;
        on_left = true;
    }
}
