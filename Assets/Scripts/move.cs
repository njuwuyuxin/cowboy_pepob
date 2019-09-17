using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour
{
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveForward = Vector2.zero;
        moveForward.x = Input.GetAxis("Horizontal");
        GetComponent<Rigidbody2D>().MovePosition(Quaternion.LookRotation(transform.forward) * moveForward * moveSpeed * Time.fixedDeltaTime + transform.position);
    }
}
