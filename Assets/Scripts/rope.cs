using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

enum RopeState {LAUNGCHING, DRAGGING, RETURNING,IDLE };
public class rope : MonoBehaviour
{
    public float MaxRopeLength = 10f;
    public float StartDragSpeed = 10f;
    public float MaxDragSpeed = 15f;
    public float AccelerateSpeed = 5f;
    public float RopeExtendSpeed = 3f;
    public float RopeReturnSpeed = 3f;
    public Vector3 startPointOffset;

    private float DragSpeed;
    private LineRenderer line;
    private bool isThrowing = true;                                 //判断是否是刚刚按下鼠标
    private RopeState RopeStatus;
    private CharacterController2D _controller;
    private move _moveScript;
    private Vector2 rayDirection;                                       //绳子发射过程中，用来检测是否勾到物体的射线（方向向量）
    private Vector2 rayHitPoint;                                        //绳子末端发射的极短射线的碰撞点，可以看作绳索勾到的点的坐标
    private Vector2 _veclocity;
    private Vector2 ropeStartPoint;                                  //绳子当前帧终点坐标
    private Vector2 ropeEndPoint;                                   //绳子当前帧终点坐标

    public Animator anim;//动画管理器（动画）
    // Start is called before the first frame update
    void Start()
    {
        RopeStatus = RopeState.IDLE;
        line = GetComponent<LineRenderer>();
        _controller = GetComponent<CharacterController2D>();
        _moveScript = GetComponent<move>();
        _veclocity = new Vector2(0f, 0f);
        DragSpeed = StartDragSpeed;
        ropeStartPoint = transform.position + startPointOffset;
        ropeEndPoint = ropeStartPoint;
        anim = GetComponent<Animator>();//获取Animator部件（动画）
    }

    // Update is called once per frame
    void Update()
    {
        //保证绳子起始位置偏移量和人物朝向一致
        if ((transform.localScale.x > 0 && startPointOffset.x < 0)||(transform.localScale.x < 0 && startPointOffset.x > 0)) 
            startPointOffset.x = -startPointOffset.x;

        if (Input.GetMouseButtonDown(1)&&RopeStatus==RopeState.IDLE)
        {
            RopeStatus = RopeState.LAUNGCHING;
            if (isThrowing)
            {
                anim.SetBool("isThrowingRope", true);
                isThrowing = false;
            }
            //如果可以空扔绳子就用这个
            Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 MousePosition2D = new Vector2(MousePosition.x, MousePosition.y);

            //根据鼠标点击位置更改人物朝向
            if (MousePosition2D.x > transform.position.x && transform.localScale.x < 0)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,transform.localScale.z);
            if (MousePosition2D.x < transform.position.x && transform.localScale.x > 0)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            //Debug.Log(MousePosition2D);

            ropeStartPoint = transform.position + startPointOffset;
            ropeEndPoint = ropeStartPoint;
            //按下按键时，一次性确定射线方向，该方向在发射过程中会用到
            rayDirection = new Vector2(MousePosition2D.x - ropeStartPoint.x, MousePosition2D.y - ropeStartPoint.y);
            rayDirection.Normalize();
            line.enabled = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (RopeStatus == RopeState.LAUNGCHING)
                RopeStatus = RopeState.RETURNING;
            if (RopeStatus == RopeState.DRAGGING)
            {
                RopeStatus = RopeState.IDLE;
                _moveScript.enabled = true;
                _moveScript._velocity.x = _veclocity.x;
                _moveScript._velocity.y = _veclocity.y;
                _veclocity.x = 0f;
                _veclocity.y = 0f;
                DragSpeed = StartDragSpeed;
            }
                
            anim.SetBool("isDragging",false);
            anim.SetBool("isThrowingRope", false);
            isThrowing = true;
        }

        if (RopeStatus==RopeState.LAUNGCHING)
        {
            ropeStartPoint = transform.position + startPointOffset;
            //暂时先用这种方式延伸绳子
            ropeEndPoint += rayDirection * Time.deltaTime * RopeExtendSpeed;
            //如果绳子已达最大长度
            if ((ropeEndPoint - ropeStartPoint).magnitude > MaxRopeLength)
            {
                RopeStatus = RopeState.RETURNING;
                return; 
                //直接结束本次Update函数
            }

            //为保证钩锁勾不同距离的位置，发射速度保持一致，插值时的百分比（第三个参数）除以一个绳子距离（以刚发射时为准）来保持匀速（该方法已弃用）
            //LerpPercent += Time.deltaTime;
            //ropeEndPoint = Vector2.Lerp(ropeStartPoint, rayHitPoint, LerpPercent*RopeExtendSpeed/RopeDistance);


            Ray2D testRay = new Ray2D(ropeEndPoint, rayDirection);
            Debug.DrawRay(testRay.origin, testRay.direction, Color.blue);
            RaycastHit2D info = Physics2D.Raycast(ropeEndPoint, rayDirection, 0.5f);
            if (info.collider != null)
            {
                //如果发生了碰撞，则说明勾到物体
                Debug.Log(info.point);
                rayHitPoint = info.point;
                RopeStatus = RopeState.DRAGGING;
                anim.SetBool("isThrowingRope", false);
                anim.SetBool("isDragging", true);
            }

            line.SetPosition(0,ropeStartPoint);
            line.SetPosition(1, ropeEndPoint);
            line.material.color = Color.blue;
        }
        else if (RopeStatus==RopeState.DRAGGING)
        {
            _moveScript.enabled = false;

            //如果已经到达目标点，则钩锁自动脱离
            ropeStartPoint = transform.position + startPointOffset;
            ropeEndPoint = rayHitPoint;
            if (Mathf.Abs(rayHitPoint.x - ropeStartPoint.x) < 0.5f && Mathf.Abs(rayHitPoint.y - ropeStartPoint.y) < 0.5f)
            {
                RopeStatus = RopeState.IDLE;
                _moveScript.enabled = true;
                _moveScript._velocity.x = _veclocity.x;
                _moveScript._velocity.y = _veclocity.y;
                _veclocity.x = 0f;
                _veclocity.y = 0f;
                DragSpeed = StartDragSpeed;
                anim.SetBool("isDragging",false);
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
        else if (RopeStatus == RopeState.RETURNING)
        {
            ropeStartPoint = transform.position + startPointOffset;
            Vector2 returnDirection = ropeEndPoint - ropeStartPoint;
            returnDirection.Normalize();
            ropeEndPoint -= returnDirection * Time.deltaTime * RopeExtendSpeed;
            line.SetPosition(0, ropeStartPoint);
            line.SetPosition(1, ropeEndPoint);
            line.material.color = Color.blue;
            if ((ropeEndPoint - ropeStartPoint).magnitude < 0.5f)
            {
                ropeEndPoint = ropeStartPoint;
                RopeStatus = RopeState.IDLE;
            }
        }
        else if(RopeStatus == RopeState.IDLE)
        {
            ropeStartPoint = transform.position + startPointOffset;
            if ((ropeEndPoint - ropeStartPoint).magnitude > 0.5f)
            {
                //Debug.Log("return ropen when idle");
                //Debug.Log("startpoint=" + ropeStartPoint);
                //Debug.Log("endpoint=" + ropeEndPoint);
                Vector2 returnDirection = ropeEndPoint - ropeStartPoint;
                returnDirection.Normalize();
                ropeEndPoint -= returnDirection * Time.deltaTime * RopeExtendSpeed;
                line.SetPosition(0, ropeStartPoint);
                line.SetPosition(1, ropeEndPoint);
                line.material.color = Color.blue;
            }
            else
            {
                line.enabled = false;
            }
        }
    }
}
