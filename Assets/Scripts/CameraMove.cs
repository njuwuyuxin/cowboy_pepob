using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform FollowObject;
    public float BorderLeft=0;
    public float BorderRight=10000;
    public float BorderBottom=0;
    public float BorderTop=4;
    private Transform CameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        CameraTransform =  GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (FollowObject.position.x < BorderLeft)
        {
            CameraTransform.position = new Vector3(BorderLeft, CameraTransform.position.y,-10f);
        }
        else if (FollowObject.position.x > BorderRight)
        {
            CameraTransform.position = new Vector3(BorderRight, CameraTransform.position.y, -10f);
        }
        else
        {
            CameraTransform.position = new Vector3(FollowObject.position.x, CameraTransform.position.y,-10f);
        }

        if (FollowObject.position.y < BorderBottom)
        {
            CameraTransform.position = new Vector3(CameraTransform.position.x, BorderBottom, -10f);
        }
        else if (FollowObject.position.y > BorderTop)
        {
            CameraTransform.position = new Vector3(CameraTransform.position.x, BorderTop, -10f);
        }
        else
        {
            CameraTransform.position = new Vector3(CameraTransform.position.x, FollowObject.position.y, -10f);
        }
    }
}
