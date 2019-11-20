using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Call_Elevator : MonoBehaviour
{
    public Elevator Elevator;
    public int number;
    public GameObject InteractiveUI;
    private GameObject UIInstance;
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
            if (InteractiveUI != null)
            {
                Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
                UIInstance = Instantiate(InteractiveUI);
                Transform temp2 = UIInstance.transform.GetChild(0);
                Vector2 offset = new Vector2(10, 25);
                temp2.position = screenPos + offset;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            IsIn = false;
            if (UIInstance != null)
                Destroy(UIInstance);
        }
    }
}
