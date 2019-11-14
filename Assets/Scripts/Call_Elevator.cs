using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Call_Elevator : MonoBehaviour
{
    public Elevator Elevator;
    public int number;
    //public Vector2 End_Position;
    bool IsIn=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(IsIn)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Elevator.Call_Elevator(number);
            }
        }
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            Debug.Log("call_elevator");
            IsIn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            IsIn = false;
        }
    }
}
