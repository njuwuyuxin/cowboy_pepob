using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum GameState { RUN,PAUSE};

public class GameManager : MonoBehaviour
{
    public static GameManager _GameManager = null; //单例模式
    public GameObject PauseMenu;
    public GameObject DieMenu;

    private GameState GameStatus;
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
        GameStatus = GameState.RUN;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Escape)&&GameStatus==GameState.RUN)
        {
            Time.timeScale = 0;
            GameStatus = GameState.PAUSE;
            Debug.Log("get esc down when run");
            Instantiate(PauseMenu);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && GameStatus == GameState.PAUSE)
        {
            Time.timeScale = 1;
            GameStatus = GameState.RUN;
            Debug.Log("get esc down when pause");
            GameObject temp = GameObject.FindGameObjectWithTag("PauseMenu");
            Destroy(temp);
            return;
        }
    }

    public void LoadSceneByName(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void ResumGame()
    {
        Time.timeScale = 1;
        _GameManager.GameStatus = GameState.RUN;
        Debug.Log(GameStatus);
        GameObject temp = GameObject.FindGameObjectWithTag("PauseMenu");
        Destroy(temp);
    }

    public void ExitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        Instantiate(DieMenu);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        LoadSceneByName("MainMenu");
    }
}
