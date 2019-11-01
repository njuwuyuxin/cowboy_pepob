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
    private float AlreadyMovedDistance;           //每次移动时，已经移动的距离
    private PositionState PositionStatus;           //电梯当前位置状态，处于起点or处于终点
    private ActionState ActionStatus;                //电梯当前动作状态，静止or正在移动
    private GameObject Player;                         //用来记录与电梯发生接触的主角物体
    // Start is called before the first frame update
    void Awake()
    {
        PositionStatus = PositionState.START;
        AlreadyMovedDistance = 0;
        ActionStatus = ActionState.IDLE;
        Player = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (ActionStatus == ActionState.MOVING)
        {
            if (ElevatorDirection == 1||ElevatorDirection==-1)
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
                    distance = (dist - (AlreadyMovedDistance - MoveDistance)) * ElevatorDirection;

                    ActionStatus= ActionState.IDLE;
                    AlreadyMovedDistance = 0;
                    if (PositionStatus == PositionState.START)
                        PositionStatus = PositionState.END;
                    else
                        PositionStatus = PositionState.START;
                    Debug.Log("Elevator Move Finish!");
                }

                if (PositionStatus == PositionState.END)        //如果电梯处于终点，那么需要反向移动。
                    distance = -distance;
                Vector3 distVector = new Vector3(0,distance,0);

                
                transform.Translate(distVector);
                Player.GetComponent<CharacterController2D>().move(distVector);
            }
        }
       
    }

    public void ElevatorStart(GameObject player)
    {
        if (ActionStatus == ActionState.IDLE)
        {
            ActionStatus = ActionState.MOVING;
            Debug.Log("Elevator Start!");
            Player = player;
        }
        else
        {
            Debug.Log("Elevator is moving!");
        }    
    }
}
