using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
enum PositionState { START,END};                   //目前实现两层楼之间的电梯，若要多层需要增加状态
enum ActionState { IDLE,MOVING};

public class Elevator : MonoBehaviour
{
    public int ElevatorDirection = 1;                   //1代表纵向向上电梯，-1代表纵向向下电梯，2代表横向向右电梯，-2代表横向向左电梯
    public float MoveDistance;                           //电梯启动一次移动的距离
    public float ElevatorSpeed;
    public bool isAuto = false;                            //默认非自动电梯，需要玩家手动启动。 若为自动电梯，则在限定范围内往复移动
    public int rayCount = 8;                                //发射射线条数，可自定义

    private float AlreadyMovedDistance;           //每次移动时，已经移动的距离
    private PositionState PositionStatus;           //电梯当前位置状态，处于起点or处于终点
    private ActionState ActionStatus;                //电梯当前动作状态，静止or正在移动
    private GameObject Player;                         //用来记录与电梯发生接触的主角物体
    private RaycastHit2D _raycastHit;
    private Vector3 ElevatorSize;
    // Start is called before the first frame update
    void Awake()
    {
        PositionStatus = PositionState.START;
        AlreadyMovedDistance = 0;
        ActionStatus = ActionState.IDLE;
        Player = null;

        if(isAuto)
            ElevatorStart();
        ElevatorSize = gameObject.GetComponent<Renderer>().bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        bool HitFlag = false;
        Vector2 rayDirection = new Vector2(0, 1);
        for (int i = 0; i < rayCount; i++)
        {
            Vector3 startPoint = transform.position;
            startPoint.x = startPoint.x - ElevatorSize.x / 2 + i * (ElevatorSize.x / (rayCount - 1));
            _raycastHit = Physics2D.Raycast(startPoint, rayDirection, 0.55f);
            //Debug.DrawRay(startPoint, rayDirection);
            if (_raycastHit)
            {
                //Debug.Log(_raycastHit.collider.tag);
                Player = _raycastHit.collider.gameObject;
                HitFlag = true;
            }
        }
        if (HitFlag == false)
        {
            Player = null;  //没有检测到上面有主角时，主角物体置空
            //Debug.Log("Nothing hit");
        }

        if (ActionStatus == ActionState.MOVING)
        {
            //if (ElevatorDirection == 1||ElevatorDirection==-1)
            {
                float dist = ElevatorSpeed * Time.deltaTime;
                AlreadyMovedDistance += dist;
                float distance;
                if (AlreadyMovedDistance < MoveDistance)
                {
                    distance = dist * ElevatorDirection;
                }
                else                //该帧时电梯已经到达终点
                {
                    int Directionflag = 1;
                    if (ElevatorDirection > 0)
                        Directionflag = 1;
                    else
                        Directionflag = -1;
                    distance = (dist - (AlreadyMovedDistance - MoveDistance)) * Directionflag;

                    ActionStatus= ActionState.IDLE;
                    AlreadyMovedDistance = 0;
                    if (PositionStatus == PositionState.START)
                        PositionStatus = PositionState.END;
                    else
                        PositionStatus = PositionState.START;
                    Debug.Log("Elevator Move Finish!");

                    if (isAuto)         //如果是自动电梯，结束后会自动开始新一轮自动移动
                        ElevatorStart();
                }

                if (PositionStatus == PositionState.END)        //如果电梯处于终点，那么需要反向移动。
                    distance = -distance;
                Vector3 distVector;
                if (ElevatorDirection == 1 || ElevatorDirection == -1)
                    distVector = new Vector3(0, distance, 0);
                else
                    distVector = new Vector3(distance, 0, 0);
                transform.Translate(distVector);
                if(Player!=null)
                    Player.GetComponent<CharacterController2D>().move(distVector);
            }
        }
       
    }

    public void ElevatorStart()
    {
        if (ActionStatus == ActionState.IDLE)
        {
            ActionStatus = ActionState.MOVING;
            Debug.Log("Elevator Start!");
        }
        else
        {
            Debug.Log("Elevator is moving!");
        }    
    }

    void OnColliderEnter2D(Collider2D col)
    {
        if(col.tag=="Player")
            Debug.Log("Player on Elevator");
    }

    void OnColliderExit2D(Collider2D col)
    {
        if (col.tag == "Player")
            Debug.Log("Player left Elevator");
    }
}
