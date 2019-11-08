using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class button_collapse_floor : MonoBehaviour
{
    public int rayCount = 4;                         //用于定义玩家站上地板后，经过多长时间地板开始塌陷
    public float FallingSpeed = 3;                      //地板塌陷下落速度
    public float TimeBeforeDestroy = 1;          //用于定义地板开始塌陷后，经过多长时间会销毁自身
    private FloorState FloorStatus;
    private RaycastHit2D _raycastHit;
    private Vector3 FloorSize;
    public CollapseFloor[] CollapseFloors;
    // Start is called before the first frame update
    void Start()
    {
        FloorStatus = FloorState.NORMAL;

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
                    FloorStatus = FloorState.FALLING;
                    break;
                }
            }
        }
        if (FloorStatus == FloorState.FALLING)
        {
            Vector3 velocity = new Vector3(0, -FallingSpeed, 0);
            transform.Translate(velocity * Time.deltaTime);
            Destroy(gameObject, TimeBeforeDestroy);
            for (int i=0;i< CollapseFloors.Length;i++)
            {
                CollapseFloors[i].button_call();
            }
        }
    }

}
