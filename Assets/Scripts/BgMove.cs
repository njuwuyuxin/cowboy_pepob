using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMove : MonoBehaviour
{
    public GameObject Camera;
    private Vector3 _velocity;
    private Vector3 DisplacementThisFrame;
    private CameraMove cameraMoveScript;
    // Start is called before the first frame update
    void Start()
    {
        cameraMoveScript = Camera.GetComponent<CameraMove>();
        if (cameraMoveScript == null)
            Debug.Log("no camera move script");
    }

    // Update is called once per frame
    void Update()
    {
        //_velocity = cameraMoveScript.CameraVelocity*0.7f;
        //_velocity.y = cameraMoveScript.CameraVelocity.y * 0.3f;
        //if (cameraMoveScript == null)
        //    Debug.Log("no camera move script");
        DisplacementThisFrame = cameraMoveScript.DisplacementThisFrame * 0.7f;
        DisplacementThisFrame.y = cameraMoveScript.DisplacementThisFrame.y * 0.3f;
        //transform.position += _velocity * Time.deltaTime;
        transform.position += DisplacementThisFrame;
    }
}
