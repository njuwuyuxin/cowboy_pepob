﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class rope : MonoBehaviour
{
    public float MaxRopeLength = 10f;
    public float StartDragSpeed = 10f;
    public float MaxDragSpeed = 15f;
    public float AccelerateSpeed = 5f;
    public float RopeExtendSpeed = 3f;
    public Vector3 startPointOffset;


    private float DragSpeed;
    private LineRenderer line;
    private bool isThrowing = true;                                 //判断是否是刚刚按下鼠标
    private bool isDragging;
    private bool isLaunching;
    private CharacterController2D _controller;
    private move _moveScript;
    private rope _ropeScript;
    private Vector2 rayHitPoint;
    private Vector2 _veclocity;
    private Vector2 ropeStartPoint;                                  //绳子当前帧终点坐标
    private Vector2 ropeEndPoint;                                   //绳子当前帧终点坐标（用来计算是否勾到物体）
    private Vector2 ropeEndPointLastFrame;                  //绳子上一帧终点坐标（用来计算是否勾到物体）
    private float LerpPercent;                                          //绳子延伸时的插值百分比
    private float RopeDistance;                                       //绳子刚勾到物体时，绳子的长度（目标点到绳子起始点）

    public Animator anim;//动画管理器（动画）
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        isDragging = false;
        isLaunching = false;
        _controller = GetComponent<CharacterController2D>();
        _moveScript = GetComponent<move>();
        _ropeScript = GetComponent<rope>();
        _veclocity = new Vector2(0f, 0f);
        DragSpeed = StartDragSpeed;
        anim = GetComponent<Animator>();//获取Animator部件（动画）
    }

    // Update is called once per frame
    void Update()
    {
        //保证绳子起始位置偏移量和人物朝向一致
        if ((transform.localScale.x > 0 && startPointOffset.x < 0)||(transform.localScale.x < 0 && startPointOffset.x > 0)) 
            startPointOffset.x = -startPointOffset.x;

        if (Input.GetMouseButtonDown(1))
        {
            if(isThrowing)
            {
                anim.SetTrigger("RopeOut");
                isThrowing = false;
            }
            Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 MousePosition2D = new Vector2(MousePosition.x, MousePosition.y);

            //根据鼠标点击位置更改人物朝向
            if (MousePosition2D.x > transform.position.x && transform.localScale.x < 0)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,transform.localScale.z);
            if (MousePosition2D.x < transform.position.x && transform.localScale.x > 0)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            //Debug.Log(MousePosition2D);

            Vector2 RayDirectrion = new Vector2(MousePosition2D.x - transform.position.x, MousePosition2D.y - transform.position.y);
            Ray2D testRay = new Ray2D(transform.position, RayDirectrion);
            Debug.DrawRay(testRay.origin, testRay.direction, Color.blue);

            RaycastHit2D info = Physics2D.Raycast(transform.position, RayDirectrion, MaxRopeLength);
            if (info.collider != null)
            {
                //如果发生了碰撞
                Debug.Log(info.point);
                isLaunching = true;

                //绳子发射起始位置增加一个偏移量，保证视觉上绳子从手部发射（以人物中心为起点）
                ropeStartPoint = transform.position + startPointOffset;
                Debug.Log("ropeStartPoint with offset:"+ropeStartPoint);

                ropeEndPoint = ropeStartPoint;
                LerpPercent = 0;
                line.enabled = true;
                rayHitPoint = info.point;

                RopeDistance = (rayHitPoint-ropeStartPoint).magnitude;
                
                //GameObject obj = info.collider.gameObject;
                //if (obj.CompareTag("Building"))//用tag判断碰到了什么对象
                //{
                //}
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            isLaunching = false;
            isDragging = false;
            isThrowing = true;
        }

        if (isLaunching)
        {
            ropeStartPoint = transform.position + startPointOffset;
            Vector2 forceDirection = new Vector2(rayHitPoint.x - ropeStartPoint.x, rayHitPoint.y - ropeStartPoint.y);
            forceDirection.Normalize(); //方向向量
            ropeEndPointLastFrame = ropeEndPoint;
            //ropeEndPoint += forceDirection * Time.deltaTime*RopeExtendSpeed;
            //Debug.Log("DeltaTime:");
            //Debug.Log(Time.deltaTime);
            LerpPercent += Time.deltaTime;

            //为保证钩锁勾不同距离的位置，发射速度保持一致，插值时的百分比（第三个参数）除以一个绳子距离（以刚发射时为准）来保持匀速
            ropeEndPoint = Vector2.Lerp(ropeStartPoint, rayHitPoint, LerpPercent*RopeExtendSpeed/RopeDistance);


            if (rayHitPoint.x - ropeEndPointLastFrame.x >= 0 && rayHitPoint.x - ropeEndPoint.x <= 0)
            {
                isLaunching = false;
                isDragging = true;
                anim.SetTrigger("RopeIn");
            }
            if (rayHitPoint.x - ropeEndPointLastFrame.x <= 0 && rayHitPoint.x - ropeEndPoint.x >= 0)
            {
                isLaunching = false;
                isDragging = true;
                anim.SetTrigger("RopeIn");
            }
            if (rayHitPoint.y - ropeEndPointLastFrame.y >= 0 && rayHitPoint.y - ropeEndPoint.y <= 0)
            {
                isLaunching = false;
                isDragging = true;
                anim.SetTrigger("RopeIn");
            }
            if (rayHitPoint.y - ropeEndPointLastFrame.y <= 0 && rayHitPoint.y - ropeEndPoint.y >= 0)
            {
                isLaunching = false;
                isDragging = true;
                anim.SetTrigger("RopeIn");
            }
            line.SetPosition(0,ropeStartPoint);
            line.SetPosition(1, ropeEndPoint);
            line.material.color = Color.blue;
        }
        else if (isDragging)
        {
            _moveScript.enabled = false;
            //如果已经到达目标点，则钩锁自动脱离
            ropeStartPoint = transform.position + startPointOffset;
            if (Mathf.Abs(rayHitPoint.x - ropeStartPoint.x) < 0.5f && Mathf.Abs(rayHitPoint.y - ropeStartPoint.y) < 0.5f)
            {
                isDragging = false;
                anim.SetTrigger("LandSafe");
            }

            line.SetPosition(0, ropeStartPoint);
            line.SetPosition(1, rayHitPoint);
            line.material.color = Color.blue;

            Vector2 forceDirection = new Vector2(rayHitPoint.x - ropeStartPoint.x, rayHitPoint.y - ropeStartPoint.y);
            forceDirection.Normalize(); //方向向量
            //Debug.Log(forceDirection);
            _veclocity = forceDirection * DragSpeed;
            //Debug.Log(DragSpeed);

            if (DragSpeed < MaxDragSpeed)
                DragSpeed += AccelerateSpeed * Time.deltaTime;
            _controller.move(_veclocity * Time.deltaTime);
        }
        else
        {
            _moveScript.enabled = true;
            if (line.enabled == true)
            {
                _moveScript._velocity.x = _veclocity.x;
                _moveScript._velocity.y = _veclocity.y;
            }
            
            line.enabled = false;
            _veclocity.x = 0f;
            _veclocity.y = 0f;
            DragSpeed = StartDragSpeed;
        }
    }
}
