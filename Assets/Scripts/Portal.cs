using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public string SceneName;                        //要传送到的新的场景的名字
    public Vector2 CharactorPosition;           //主角从该点传送到新场景后，在新场景中所处坐标
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.tag);
        if(col.tag=="Player")
        {
            GameManager._GameManager.LoadSceneByName(SceneName);
            col.transform.position = new Vector3(CharactorPosition.x, CharactorPosition.y, col.transform.position.z);
        }
    }
}
