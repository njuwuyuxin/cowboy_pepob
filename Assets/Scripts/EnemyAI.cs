﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEditor;

enum EnemyState{ PATROL, TRACK,FIRE  };
public class EnemyAI : MonoBehaviour
{
    public Transform target;
    private EnemyState state = EnemyState.PATROL;
    public float speed = 200f;
    public float nextWayPointDistance = 3f;

    public Transform enemyGFX;
    Path path;
    int currentWayPoint = 0;
    bool reachedEndOfPath=false;
    Seeker seeker;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        //重复寻路/0.1s
        InvokeRepeating("UpdatePath", 0f, .8f);
    }
    //视野角度
    [Range(0, 180)]
    public int viewAngle;
    //视野半径
    public float viewRadius = 8;
    //朝向
    private Vector3 forward = new Vector3(-1f, 0f, 0f);
    private Vector3 up = new Vector3(0f, 0f, 1f);
    void OnDrawGizmos()
    {
        Color color = Handles.color;
        Handles.color = Color.red;
        int angle = viewAngle / 2;
        //绕z轴旋转半个
        Vector3 startLine = Quaternion.Euler(0,0 , -angle) * forward;
        Handles.DrawSolidArc(this.enemyGFX.position, up, new Vector3(startLine.x, startLine.y, 0f), viewAngle, viewRadius);
        Handles.color = color;
    }
    float getAngle(Vector3 fromVector, Vector3 toVector) {
        float angle = Vector3.Angle(fromVector, toVector);
        Vector3 normal = Vector3.Cross(fromVector, toVector);
        angle *= Mathf.Sign(Vector3.Dot(normal, up));
        return angle;
    }
    void UpdatePath() {
        if(state != EnemyState.PATROL && seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }
    void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWayPoint = 0;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state) {
            case EnemyState.PATROL:
                Debug.Log("PATROL");
                float rangerSqr = (this.enemyGFX.position - target.position).sqrMagnitude;
                //距离小于视野
                if (rangerSqr <= viewRadius * viewRadius)
                {
                    Vector3 sight = target.position - this.enemyGFX.position;
                    float angle=getAngle(new Vector3(sight.x, sight.y, 0), forward);
                    //在视野里或者实在太近
                    if (angle < viewAngle / 2 && angle > -viewAngle / 2 || rangerSqr<= viewRadius) {
                        lastStartTrack = 0f;
                        state = EnemyState.TRACK;
                    }
                }
                break;
            case EnemyState.TRACK:
                Debug.Log("TRACK");
                track();
                //至少追踪五秒后，并且离开根号2 *视野后停止最终
                if ((lastStartTrack>5f && (this.enemyGFX.position - target.position).sqrMagnitude >= 2 * viewRadius * viewRadius)) {
                    state = EnemyState.PATROL;
                }
                break;
            case EnemyState.FIRE:
                break;
        }





    }

    float lastStartTrack = 0;
    void track()
    {
        if (path == null)
        {
            return;
        }
        if (currentWayPoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
        }
        else
        {
            reachedEndOfPath = false;
        }
        //得到施加力的方向，记得加线性阻力，否则不会停
        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        lastStartTrack += Time.deltaTime;


        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }
        //转身
        if (force.x >= 0.01f)
        {
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
            forward = new Vector3(1f, 0f, 0f);


        }
        else if (force.x <= -0.01f)
        {
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
            forward = new Vector3(-1f, 0f, 0f);
        }
        rb.AddForce(force);
    }
}
