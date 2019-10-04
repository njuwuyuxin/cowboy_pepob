using System.Collections;
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


    private float DragSpeed;
    private LineRenderer line;
    private bool isDragging;
    private bool isLaunching;
    private CharacterController2D _controller;
    private move _moveScript;
    private rope _ropeScript;
    private Vector2 rayHitPoint;
    private Vector2 _veclocity;
    private Vector2 ropeStartPoint;
    private Vector2 ropeEndPoint;
    private Vector2 ropeEndPointLastFrame;
    private float LerpPercent;
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
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(transform.position);
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 MousePosition2D = new Vector2(MousePosition.x, MousePosition.y);
            //Debug.Log(MousePosition2D);

            Vector2 RayDirectrion = new Vector2(MousePosition2D.x - transform.position.x, MousePosition2D.y - transform.position.y);
            Ray2D testRay = new Ray2D(transform.position, RayDirectrion);
            Debug.DrawRay(testRay.origin, testRay.direction, Color.blue);

            RaycastHit2D info = Physics2D.Raycast(transform.position, RayDirectrion, MaxRopeLength);
            if (info.collider != null)
            {
                //如果发生了碰撞
                Debug.Log(info.point);
                //isDragging = true;
                isLaunching = true;
                ropeStartPoint = transform.position;
                ropeEndPoint = transform.position;
                LerpPercent = 0;
                line.enabled = true;
                rayHitPoint = info.point;
                
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
        }

        if (isLaunching)
        {
            Vector2 forceDirection = new Vector2(rayHitPoint.x - transform.position.x, rayHitPoint.y - transform.position.y);
            forceDirection.Normalize(); //方向向量
            ropeEndPointLastFrame = ropeEndPoint;
            //ropeEndPoint += forceDirection * Time.deltaTime*RopeExtendSpeed;
            //Debug.Log("DeltaTime:");
            //Debug.Log(Time.deltaTime);
            LerpPercent += Time.deltaTime;
            ropeEndPoint = Vector2.Lerp(ropeStartPoint, rayHitPoint, LerpPercent*RopeExtendSpeed);


            if (rayHitPoint.x - ropeEndPointLastFrame.x >= 0 && rayHitPoint.x - ropeEndPoint.x <= 0)
            {
                isLaunching = false;
                isDragging = true;
            }
            if (rayHitPoint.x - ropeEndPointLastFrame.x <= 0 && rayHitPoint.x - ropeEndPoint.x >= 0)
            {
                isLaunching = false;
                isDragging = true;
            }
            if (rayHitPoint.y - ropeEndPointLastFrame.y >= 0 && rayHitPoint.y - ropeEndPoint.y <= 0)
            {
                isLaunching = false;
                isDragging = true;
            }
            if (rayHitPoint.y - ropeEndPointLastFrame.y <= 0 && rayHitPoint.y - ropeEndPoint.y >= 0)
            {
                isLaunching = false;
                isDragging = true;
            }
            //if (Mathf.Abs(rayHitPoint.x - ropeEndPoint.x) < 0.6f && Mathf.Abs(rayHitPoint.y - ropeEndPoint.y) < 0.6f)
            //{
            //    isLaunching = false;
            //    isDragging = true;
            //}
            line.SetPosition(0, transform.position);
            line.SetPosition(1, ropeEndPoint);
            line.material.color = Color.blue;
        }
        else if (isDragging)
        {
            _moveScript.enabled = false;
            //如果已经到达目标点，则钩锁自动脱离
            if (Mathf.Abs(rayHitPoint.x - transform.position.x) < 0.5f && Mathf.Abs(rayHitPoint.y - transform.position.y) < 0.5f)
                isDragging = false;

            line.SetPosition(0, transform.position);
            line.SetPosition(1, rayHitPoint);
            line.material.color = Color.blue;

            Vector2 forceDirection =new Vector2( rayHitPoint.x - transform.position.x,rayHitPoint.y-transform.position.y);
            forceDirection.Normalize(); //方向向量
            Debug.Log(forceDirection);
            _veclocity = forceDirection * DragSpeed;
            Debug.Log(DragSpeed);

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
