using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class be_shot_wall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.tag);
        if(collision.tag=="Bullet")
        {
            Destroy(gameObject, 0.5f);
        }
    }
}
