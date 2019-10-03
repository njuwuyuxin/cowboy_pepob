using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

public class rope : MonoBehaviour
{
    public float StartDragSpeed = 10f;
    public float MaxDragSpeed = 15f;
    public float AccelerateSpeed = 5f;

    private float DragSpeed;
    private LineRenderer line;
    private bool isDragging;
    private CharacterController2D _controller;
    private move _moveScript;
    private rope _ropeScript;
    private Vector2 rayHitPoint;
    private Vector2 _veclocity;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        isDragging = false;
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

            RaycastHit2D info = Physics2D.Raycast(transform.position, RayDirectrion, 10f);
            if (info.collider != null)
            {
                //如果发生了碰撞
                Debug.Log(info.point);
                isDragging = true;
                line.enabled = true;
                rayHitPoint = info.point;
                
                GameObject obj = info.collider.gameObject;
                if (obj.CompareTag("Building"))//用tag判断碰到了什么对象
                {
                    
                    //Debug.Log(obj.name);
                    //info.collider.
                }
            }

        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            //if(_moveScript.enabled == true)
            //{

            //}
            _moveScript.enabled = false;
            //_ropeScript.enabled = true;
            //如果已经到达目标点，则钩锁自动脱离
            if (Mathf.Abs(rayHitPoint.x - transform.position.x) < 0.5f && Mathf.Abs(rayHitPoint.y - transform.position.y) < 0.5f)
                isDragging = false;

            line.SetPosition(0, transform.position);
            line.SetPosition(1, rayHitPoint);
            line.material.color = Color.blue;

            Vector2 forceDirection =new Vector2( rayHitPoint.x - transform.position.x,rayHitPoint.y-transform.position.y);
            forceDirection.Normalize(); //方向向量
            Debug.Log(forceDirection);
            //_veclocity.x += forceDirection.x * Time.deltaTime;
            //_veclocity.y += forceDirection.y * Time.deltaTime;
            _veclocity = forceDirection * DragSpeed;
            Debug.Log(DragSpeed);

            if (DragSpeed < MaxDragSpeed)
                DragSpeed += AccelerateSpeed * Time.deltaTime;
            _controller.move(_veclocity * Time.deltaTime);

        }
        else
        {
            //_ropeScript.enabled = false;
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
