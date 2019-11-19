using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject Player;
    public GameObject UICanvas;
    // Start is called before the first frame update
    void Awake()
    {
        if (GameManager._GameManager == null)
        {
            //以下加载顺序不要调整，例如UI需要先于Player加载，Player才能获取到UI相关组件
            Instantiate(gameManager);
            Instantiate(UICanvas);
            Instantiate(Player,new Vector3(1,0,-2),new Quaternion(0,0,0,0));

            GameManager._GameManager.LoadSceneByName("Factory1");           //初始场景入口，如需调整初始场景调试，需在此处修改
        }
    }
}
