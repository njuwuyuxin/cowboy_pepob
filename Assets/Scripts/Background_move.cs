using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_move : MonoBehaviour
{
    // Start is called before the first frame update
    public  float movespeed;
    public  float background_edge_x;
    public  float camera_edge_x;
    GameObject maincamera;
    // Start is called before the first frame update
    void Start()
    {
        movespeed = 3.0f;
        maincamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * movespeed * Time.deltaTime);
        Vector3 position = transform.position;
        if (position.x+background_edge_x <= camera_edge_x+maincamera.transform.position.x)
        {
            transform.position = new Vector3(position.x + 2*background_edge_x, position.y, position.z);
        }
    }
}
