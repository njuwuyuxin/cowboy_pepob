using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
using UnityEditor;
enum PositionState { START,END};                   //目前实现两层楼之间的电梯，若要多层需要增加状态
enum ActionState { IDLE,MOVING};

public class Elevator : MonoBehaviour
{
    public int ElevatorDirection = 1;                   //1代表纵向向上电梯，-1代表纵向向下电梯，2代表横向向右电梯，-2代表横向向左电梯
    public float ElevatorSpeed;
    public bool isAuto = false;                            //默认非自动电梯，需要玩家手动启动。 若为自动电梯，则在限定范围内往复移动
    public bool showUI = false;                          //默认不显示UI，需要显示操作说明的电梯可手动勾选
    public int rayCount = 8;                                //发射射线条数，可自定义
    public GameObject InteractiveUI;                //显示使用电梯UI
    public Vector2[] Points;
    private Vector2 Start;
    private Vector2 End;
    private int Point_number;
    private int Start_number;
    private int number_direction;
    private float AlreadyMovedDistance;           //每次移动时，已经移动的距离
    private PositionState PositionStatus;           //电梯当前位置状态，处于起点or处于终点
    private ActionState ActionStatus;                //电梯当前动作状态，静止or正在移动
    private GameObject Player;                         //用来记录与电梯发生接触的主角物体
    private float MoveDistance;                           //电梯启动一次移动的距离
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
        ElevatorSize = gameObject.GetComponent<Collider2D>().bounds.size;
        //Debug.Log(ElevatorSize);
        Start = Points[0];
        End=Points[1];
        Point_number = 1;
        Start_number = 0;
        number_direction = 1;
    }
    void OnDrawGizmos()
    {
        //Color color = Handles.color;
        //Handles.color = Color.green;
        //for(int i=0;i<Points.Length;i++)
        //    Handles.DrawSolidDisc(new Vector3(Points[i].x, Points[i].y, -1), new Vector3(0f, 0f, 1f), 0.3f);   
    }
    // Update is called once per frame
    void Update()
    {
        if(Start!=End)
        {
            _change_direction();
            bool HitFlag = false;
            Vector2 rayDirection = new Vector2(0, 1);
            for (int i = 0; i < rayCount; i++)
            {
                Vector3 startPoint = transform.position;
                startPoint.x = startPoint.x - ElevatorSize.x / 2 + i * (ElevatorSize.x / (rayCount - 1));
                _raycastHit = Physics2D.Raycast(startPoint, rayDirection, 0.52f);
                //Debug.DrawRay(startPoint, rayDirection);
                if (_raycastHit)
                {

                    Player = _raycastHit.collider.gameObject;
                    //Debug.Log(Player);
                    Player.GetComponent<move>().SetMoveStatus(1);
                    HitFlag = true;

                    if (showUI && ActionStatus == ActionState.IDLE)           //如果主角站上电梯且电梯未启动，显示UI
                    {
                        InteractiveUI.SetActive(true);
                        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
                        Transform temp2 = InteractiveUI.transform.GetChild(0);
                        Vector2 offset = new Vector2(10, 25);
                        temp2.position = screenPos + offset;
                    }
                    else
                        InteractiveUI.SetActive(false);
                }
            }
            if (HitFlag == false)
            {
                if (Player != null)
                    Player.GetComponent<move>().SetMoveStatus(0);
                Player = null;  //没有检测到上面有主角时，主角物体置空
                InteractiveUI.SetActive(false);
                //Debug.Log("Nothing hit");
            }

            if (ActionStatus == ActionState.MOVING)
            {
                MoveDistance = Mathf.Abs(End.y - Start.y) + Mathf.Abs(End.x - Start.x);
                float dist = ElevatorSpeed * Time.deltaTime;
                AlreadyMovedDistance += dist;
                float distance = dist;

                //开始计算电梯每次移动距离distance（带符号）
                if (AlreadyMovedDistance < MoveDistance)
                {
                    //distance = dist * ElevatorDirection;

                }
                else                //该帧时电梯已经到达终点
                {
                    //int Directionflag = 1;
                    //if (ElevatorDirection > 0)
                    //    Directionflag = 1;
                    //else
                    //    Directionflag = -1;
                    //distance = (dist - (AlreadyMovedDistance - MoveDistance)) * Directionflag;


                    ActionStatus = ActionState.IDLE;
                    Start = End;
                    Start_number = Point_number;
                    if (Point_number == 0)
                    {
                        Point_number = 1;
                        number_direction = 1;
                    }
                    else if (Point_number == (Points.Length - 1))
                    {
                        Point_number--;
                        number_direction = -1;
                    }
                    else if (number_direction > 0)
                    {
                        Point_number++;
                    }
                    else
                    {
                        Point_number--;
                    }
                    End = Points[Point_number];

                    //Debug.Log(Point_number);
                    AlreadyMovedDistance = 0;
                    //if (PositionStatus == PositionState.START)
                    //    PositionStatus = PositionState.END;
                    //else
                    //    PositionStatus = PositionState.START;
                    //Debug.Log("Elevator Move Finish!");

                    if (isAuto)         //如果是自动电梯，结束后会自动开始新一轮自动移动
                    {

                        ElevatorStart();
                    }
                }
                //if (PositionStatus == PositionState.END)                          //如果电梯处于终点，那么需要反向移动。
                //    distance = -distance;
                //移动距离计算完毕，开始执行电梯和主角的移动
                Vector3 distVector;
                if (ElevatorDirection == 1 || ElevatorDirection == -1)      //纵向
                {
                    if (ElevatorDirection == 1)
                        distVector = new Vector3(0, distance, 0);
                    else
                        distVector = new Vector3(0, -distance, 0);
                }
                else                                                                                    //横向
                {
                    if (ElevatorDirection == 2)
                        distVector = new Vector3(distance, 0, 0);
                    else
                        distVector = new Vector3(-distance, 0, 0);
                }
                transform.Translate(distVector);
                if (Player != null)
                    Player.GetComponent<CharacterController2D>().move(distVector);
            }
        }
    }

    public void ElevatorStart()
    {
        if (ActionStatus == ActionState.IDLE)
        {
            ActionStatus = ActionState.MOVING;
            
            //Debug.Log("Elevator Start!");
        }
        else
        {
            //Debug.Log("Elevator is moving!");
        }    
    }
    public void Call_Elevator(int des_number)
    {
        if(Start_number != des_number&& ActionStatus == ActionState.IDLE)
        {
            End = Points[des_number];
            if (Point_number < des_number)
                number_direction = -1;
            else
                number_direction = 1;
            Point_number = des_number;           
            ElevatorStart();
        }
        
    }
    void _change_direction()
    {
        if (ElevatorDirection == 1 || ElevatorDirection == -1)
        {
            
            if (End.y > Start.y)
            {
                ElevatorDirection = 1;
            }
            else
            {
                ElevatorDirection = -1;
            }
        }
        else
        {
            
            if (End.x > Start.x)
            {
                ElevatorDirection = 2;
            }
            else
            {
                ElevatorDirection = -2;
            }
        }
        //Debug.Log("end"+End.y);
        //Debug.Log("start"+Start.y);
    }
}
