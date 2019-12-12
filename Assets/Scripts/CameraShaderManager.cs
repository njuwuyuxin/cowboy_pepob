using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaderManager : MonoBehaviour
{
    public DieShaderScript dieShader;
    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartShader()
    {
        dieShader.enabled = true;
    }

    public void EndShader()
    {
        dieShader.enabled = false;
    }
}
