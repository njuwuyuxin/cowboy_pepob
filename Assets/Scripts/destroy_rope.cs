using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy_rope : MonoBehaviour
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
        if(collision.tag=="Bullet")
            Destroy(transform.GetChild(0).gameObject);
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 1;
        EdgeCollider2D edgeCollider2D = GetComponent<EdgeCollider2D>();
        edgeCollider2D.enabled = false;
    }
}
