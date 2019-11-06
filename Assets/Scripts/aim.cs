using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class aim : MonoBehaviour
{

    private CharacterController2D _controller;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //如果hand层是等待或射击状态
        //if (anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex("Hand Layer")).IsName("wait")||
        //    anim.GetCurrentAnimatorStateInfo(anim.GetLayerIndex("Hand Layer")).IsName("shoot"))
        //{
            //获取鼠标的坐标，鼠标是屏幕坐标，Z轴为0，这里不做转换，因为2D项目IK的Z轴位置无所谓  
            Vector3 mouse = Input.mousePosition;
            //转换为世界坐标
            mouse = Camera.main.ScreenToWorldPoint(mouse);
            transform.position = mouse;
        //}
    }
}
