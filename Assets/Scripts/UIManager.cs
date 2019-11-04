using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager _UIManager = null; //单例模式
    // Start is called before the first frame update
    void Awake()
    {
        if (_UIManager == null)
            _UIManager = this;
        else if (_UIManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
