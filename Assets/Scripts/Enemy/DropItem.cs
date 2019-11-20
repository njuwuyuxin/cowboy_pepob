using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PosType { Absolute,Relative};

public class DropItem : MonoBehaviour
{
    public GameObject item;     //掉落物体prefab
    public PosType PositionType;
    public Vector3 pos;      //掉落物体和敌人尸体间的位置偏移量
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
