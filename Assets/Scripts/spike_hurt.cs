using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spike_hurt : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Enemy")
        {
            
            col.gameObject.GetComponent<EnemyHealth>().Die();

            

        }
    }
        // Update is called once per frame
        void Update()
    {
        
    }
}
