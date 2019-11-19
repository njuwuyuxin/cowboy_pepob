using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_trigger : MonoBehaviour
{
    public GameObject yanti;
    // Start is called before the first frame update
    void Start()
    {
                yanti.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            
                yanti.SetActive(true);
            
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
                yanti.SetActive(false);

        }
    }
}
