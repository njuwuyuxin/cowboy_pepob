using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _GameManager = null; //单例模式
    // Start is called before the first frame update
    void Awake()
    {
        if (_GameManager == null)
            _GameManager = this;
        else if (_GameManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSceneByName(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
