using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgMove : MonoBehaviour
{
    public GameObject Camera;
    private Vector3 _velocity;
    private CameraMove cameraMoveScript;
    // Start is called before the first frame update
    void Start()
    {
        cameraMoveScript = Camera.GetComponent<CameraMove>();
    }

    // Update is called once per frame
    void Update()
    {
        _velocity = cameraMoveScript.CameraVelocity*0.7f;
        transform.position += _velocity * Time.deltaTime;
    }
}
