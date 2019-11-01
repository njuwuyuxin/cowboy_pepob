using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEditor;

enum EnemyState{ PATROL, TRACK,FIRE  };
public class EnemyAI : MonoBehaviour
{
    // 主角
    public Transform target;
    private EnemyState state = EnemyState.PATROL;
    // 敌人行动的速度
    public float speed = 200f;
    // 最后到达最终寻路点的距离（和某个寻路点小于这个距离意味着到达了这个寻路点）
    public float nextWayPointDistance = 3f;
    // 追踪的最小时间
    public float trackSecond = 5f;
    // 在视野多少秒后开火
    public float fireSecond = 1f;
    // 设置为内置的那个对象就好
    public Transform enemyGFX;
    // 设置为内置的那个对象就好
    public GameObject eneity;
    // 巡逻的开始点
    public Vector2 patrolStart;
    // 巡逻的结束点
    public Vector2 patrolEnd;
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
        InvokeRepeating("UpdatePath", 0f, 1f);
    }

    //视野角度
    [Range(0, 180)]
    public int viewAngle;
    //视野半径
    public float viewRadius = 8;
    //朝向
    private Vector3 forward = new Vector3(-1f, 0f, 0f);
    private Vector3 up = new Vector3(0f, 0f, 1f);
    private Color sightColor = Color.green;
    private bool isToEnd = true;
    void OnDrawGizmos()
    {
        Color color = Handles.color;
        Handles.color = sightColor;
        int angle = viewAngle / 2;
        //绕z轴旋转半个
        Vector3 startLine = Quaternion.Euler(0,0 , -angle) * forward;
        Handles.DrawSolidArc(this.enemyGFX.position, up, new Vector3(startLine.x, startLine.y, 0f), viewAngle, viewRadius);
        Handles.color = Color.blue;
        Handles.DrawSolidDisc(new Vector3(patrolStart.x, patrolStart.y,-1), up,0.3f);
        Handles.DrawSolidDisc(new Vector3(patrolEnd.x, patrolEnd.y, -1), up, 0.3f);
        if (state == EnemyState.PATROL || state == EnemyState.TRACK|| state == EnemyState.FIRE) {
            if (rb == null) { return; }
            RaycastHit2D hit = Physics2D.Raycast(rb.position, ((Vector2)target.position - rb.position).normalized, int.MaxValue, 1 << LayerMask.NameToLayer("OneWayPlatform"));
            Vector3 end = target.position;
            if (hit.collider!=null)
            {
                end = hit.point;
            }
            Handles.DrawBezier(rb.position, end, rb.position, end, Color.magenta, null, 3);
        }
        Handles.color = color;

    }

    float getAngle(Vector3 fromVector, Vector3 toVector) {
        float angle = Vector3.Angle(fromVector, toVector);
        Vector3 normal = Vector3.Cross(fromVector, toVector);
        angle *= Mathf.Sign(Vector3.Dot(normal, up));
        return angle;
    }
    void UpdatePath() {
        if (seeker.IsDone()) {
            if (state == EnemyState.PATROL) {
                if (isToEnd)
                {
                    seeker.StartPath(rb.position, new Vector3(patrolEnd.x, patrolEnd.y, -1), OnPathComplete);
                }
                else {
                    seeker.StartPath(rb.position, new Vector3(patrolStart.x, patrolStart.y, -1), OnPathComplete);
                }
            }else{
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }
            
        }
        
            
    }
    void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWayPoint = 0;
        }
    }
    private static Vector2 RIGHTFORWARD = new Vector2(1f, 0f);
    private static Vector2 LEFTFORWARD = new Vector2(-1f, 0f);
    // Update is called once per frame
    void FixedUpdate()
    {
        float rangerSqr = ((Vector2)this.enemyGFX.position - (Vector2)target.position).sqrMagnitude;
        
        switch (state) {
            case EnemyState.PATROL:
                //距离小于视野
                if (rangerSqr <= viewRadius * viewRadius)
                {
                    //在视野里或者实在太近
                    if (isInSight(rangerSqr)) {
                        lastStartTrack = 0f;
                        sightColor = Color.yellow;
                        state = EnemyState.TRACK;
                        //暂时追踪模式疯狂开火
                        lastInSight = 0f;
                    }
                }
                //
                if (track(speed/2)) {
                    isToEnd = !isToEnd;
                }
                break;
            case EnemyState.TRACK:
                track(speed);

                //至少追踪五秒后，并且离开 视野 后停止最终
                if ((lastStartTrack > trackSecond && !isInSight(rangerSqr)))
                {
                    lastInSight = 0f;
                    sightColor = Color.green;
                    state = EnemyState.PATROL;
                    //巡逻模式停止开火
                    eneity.SendMessage("stopFire", "");
                }
                else if (rangerSqr <= viewRadius * viewRadius&&isInSight(rangerSqr))
                {

                    if (lastInSight > fireSecond) {
                        state = EnemyState.FIRE;
                        sightColor = Color.red;
                        lastInSight = 0f;
                        eneity.SendMessage("startFire", "");
                    }
                    lastInSight += Time.deltaTime;
                }
                break;
            case EnemyState.FIRE:
                if (rangerSqr > viewRadius * viewRadius || !isInSight(rangerSqr))
                {
                    lastStartTrack = 0f;
                    lastInSight = 0f;
                    sightColor = Color.yellow;
                    state = EnemyState.TRACK;
                    eneity.SendMessage("stopFire", "");
                }
                else {
                    eneity.SendMessage("startFire", "");
                }
                break;
        }
    }
    private bool isInSight(float rangerSqr) {
        Vector3 sight = (Vector2)(target.position - this.enemyGFX.position);
        float angle = getAngle(new Vector3(sight.x, sight.y, 0), forward);
        if (angle < viewAngle / 2 && angle > -viewAngle / 2 || rangerSqr <= viewRadius)
        {
            RaycastHit2D hit = Physics2D.Raycast(rb.position, ((Vector2)target.position - rb.position).normalized, int.MaxValue, 1 << LayerMask.NameToLayer("OneWayPlatform"));
            Vector3 end = target.position;
            //视线受阻
            if (hit.collider != null)
            {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            return false;
        }
    }
    float lastStartTrack = 0;
    float lastInSight = 0;
    bool track(float speed)
    {
        if (path == null ){
            return false;
        }
        if (currentWayPoint >= path.vectorPath.Count) {
            return true;
        }
        //得到施加力的方向，记得加线性阻力，否则不会停
        if (currentWayPoint == 0) { currentWayPoint = 1; }
        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        lastStartTrack += Time.deltaTime;


        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }
        //Debug.Log(rb.velocity.magnitude);
        //转身
        if (force.x >= 1f)
        {
            enemyGFX.localScale.Set(-Mathf.Abs(enemyGFX.localScale.x), enemyGFX.localScale.y, enemyGFX.localScale.z);
            forward = RIGHTFORWARD;

        }

        else if (force.x <= -1f)
        {

            enemyGFX.localScale.Set(Mathf.Abs(enemyGFX.localScale.x), enemyGFX.localScale.y, enemyGFX.localScale.z);
            forward = LEFTFORWARD;
            
        }
        rb.AddForce(force);
        return false;
    }
}
