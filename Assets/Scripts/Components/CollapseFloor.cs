using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum FloorState { NORMAL,TRIGGERED,FALLING};          //定义地板几种状态，NORMAL=正常状态，TRIGGERED=主角踩上触发，FALLING=触发后一定时间之后开始掉落
public class CollapseFloor : MonoBehaviour
{
    public int rayCount = 4;                                //发射射线条数，可自定义
    public float TimeBeforeCollapse=1;             //用于定义玩家站上地板后，经过多长时间地板开始塌陷
    public float FallingSpeed = 3;                      //地板塌陷下落速度
    public float TimeBeforeDestroy = 1;          //用于定义地板开始塌陷后，经过多长时间会销毁自身

    private FloorState FloorStatus;
    private RaycastHit2D _raycastHit;
    private Vector3 FloorSize;
    private float TimerBeforeCollapse;              //用于计时地板塌陷前时间
    // Start is called before the first frame update
    void Awake()
    {
        FloorStatus = FloorState.NORMAL;
        TimerBeforeCollapse = 0;
        FloorSize = gameObject.GetComponent<Renderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 rayDirection = new Vector2(0, 1);
        if (FloorStatus == FloorState.NORMAL)               //只有处于空闲状态地板才进行射线检测
        {
            for (int i = 0; i < rayCount; i++)
            {
                Vector3 startPoint = transform.position;
                startPoint.x = startPoint.x - FloorSize.x / 2 + i * (FloorSize.x / (rayCount - 1));
                _raycastHit = Physics2D.Raycast(startPoint, rayDirection, 0.52f);
                Debug.DrawRay(startPoint, rayDirection);
                if (_raycastHit && _raycastHit.collider.tag == "Player")        //检测到主角在地板上
                {
                    FloorStatus = FloorState.TRIGGERED;
                    break;
                }
            }
        }

        if(FloorStatus==FloorState.TRIGGERED)
        {
            TimerBeforeCollapse += Time.deltaTime;
            if (TimerBeforeCollapse >= TimeBeforeCollapse)
            {
                FloorStatus = FloorState.FALLING;
                Destroy(gameObject,TimeBeforeDestroy);
            }
        }

        if(FloorStatus==FloorState.FALLING)
        {
            Vector3 velocity = new Vector3(0, -FallingSpeed, 0);
            transform.Translate(velocity * Time.deltaTime);
        }
    }

    //void OnColliderEnter2D(Collider2D col)
    //{
    //    Destroy(gameObject);
    //}
}
