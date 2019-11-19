using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEditor;
using Prime31;

public class GroundEnemyAI : MonoBehaviour
{
    /// <summary>
    /// 主角
    /// </summary>
    private Transform target;
    // 使用第几个寻路graph
    public int graphNum = 1;
    private EnemyState state = EnemyState.PATROL;
    // 最后到达最终寻路点的距离（和某个寻路点小于这个距离意味着到达了这个寻路点）
    public float nextWayPointDistance = 3f;
    // 追踪的最小时间
    public float trackSecond = 5f;
    // 在视野多少秒后被发现（警觉时间）
    public float toTrackSecond = 2f;
    // 在视野多少秒后开火（开火时间）
    public float toFireSecond = 1f;
    // 在单次开火的至少时间
    public float fireSecond = 1f;
    public float turnRoundSecond = 1f;
    // 设置为内置的那个对象就好
    public Transform enemyGFX;
    // 设置为内置的那个对象就好
    public GameObject eneity;
    // 巡逻的开始点
    public Vector2 patrolStart;
    // 巡逻的结束点
    public Vector2 patrolEnd;
    // 只巡逻
    public bool onlyTrack=false;
    Path path;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;
    Seeker seeker;
    Rigidbody2D rb;
    // cc（character controller）的模拟重力系数
    public float gravity = -25f;
    // cc（character controller）的奔跑速度
    public float runSpeed = 8f;
    // how fast do we change direction? higher means faster（我也不懂，问wyx）
    public float groundDamping = 20f;
    //（我也不懂，问wyx）[反正我没用过]
    public float inAirDamping = 5f;
    // 跳跃高度
    public float jumpHeight = 3f;
    // 警戒条
    public SpriteRenderer warningBar;
    private Vector3 _velocity;

    [HideInInspector]
    private float normalizedHorizontalSpeed = 0;

    private CharacterController2D _controller;
    private Animator _animator;
    private RaycastHit2D _lastControllerColliderHit;
    // 要设置状态的动画
    public Animator anim;
    private Vector3 warningFireBarScale;
    void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        warningFireBarScale = warningBar.transform.localScale;
        warningBar.material.color = Color.Lerp(Color.green, Color.red, 0);

        anim = GetComponent<Animator>();
        _controller = GetComponent<CharacterController2D>();

        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;
    }
    #region Event Listeners

    void onControllerCollider(RaycastHit2D hit)
    {
        // bail out on plain old ground hits cause they arent very interesting
        if (hit.normal.y == 1f)
            return;

        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
        //Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    }


    void onTriggerEnterEvent(Collider2D col)
    {
        //Debug.Log("onTriggerEnterEvent: " + col.gameObject.name);
    }


    void onTriggerExitEvent(Collider2D col)
    {
        //Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
    }

    #endregion
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
    private Color seeColor = Color.magenta;
    private bool isToEnd = true;
    private bool toEnd = false;
    private bool isRunning = false;
    void OnDrawGizmos()
    {
        //Color color = Handles.color;
        //Handles.color = sightColor;
        //int angle = viewAngle / 2;
        ////绕z轴旋转半个
        //Vector3 startLine = Quaternion.Euler(0, 0, -angle) * forward;
        //Handles.DrawSolidArc(this.enemyGFX.position, up, new Vector3(startLine.x, startLine.y, 0f), viewAngle, viewRadius);
        //Handles.color = Color.blue;
        //Handles.DrawSolidDisc(new Vector3(patrolStart.x, patrolStart.y, -1), up, 0.3f);
        //Handles.DrawSolidDisc(new Vector3(patrolEnd.x, patrolEnd.y, -1), up, 0.3f);
        //if (state == EnemyState.PATROL || state == EnemyState.TRACK || state == EnemyState.FIRE)
        //{
        //    if (rb == null) { return; }
        //    if ((rb.position- (Vector2)target.position).sqrMagnitude<=4*viewRadius*viewRadius) {
        //        Vector2 end = target.position;
        //        bool canSee = getSightPoint(rb.position, target.position, out end, "ground", "Default");
        //        if (canSee)
        //        {
        //            seeColor = Color.red;
        //        }
        //        if((end- rb.position).sqrMagnitude>viewRadius)
        //            Handles.DrawBezier(rb.position, end, rb.position, end, seeColor, null, 3);

        //        seeColor = Color.magenta;
        //    }

        //}
        //if (isRunning)
        //{
        //    if (rb == null) { return; }
        //    float x = 1 * forward.x;
        //    int y = 1;
        //    Vector2 start = new Vector2(rb.position.x + x, rb.position.y + y);
        //    RaycastHit2D hit = Physics2D.Raycast(start, new Vector2(0, -1f), 8, 1 << LayerMask.GetMask("ground") | 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("OneWayPlatform"));
        //    Vector3 end = new Vector2(rb.position.x + x, rb.position.y + y - 8);
        //    if (hit.collider != null)
        //    {
        //        end = hit.point;
        //    }
        //    Handles.DrawBezier(start, end, start, end, Color.magenta, null, 3);
        //}

        //Handles.color = color;

    }
    bool getSightPoint(Vector2 enemyPos,Vector2 playerPos, out Vector2 colliderPos, params string[] layers) {
        
        float dis = (playerPos - enemyPos).sqrMagnitude;
        Vector3 sight = (playerPos - enemyPos);
        float angle = getAngle(new Vector3(sight.x, sight.y, 0), forward);
        int layersMask=0;
        if (layers.Length > 0) {
            layersMask = 1 << LayerMask.GetMask(layers[0]);
            for (int i = 1; i < layers.Length; i++) {
                layersMask = layersMask | 1 << LayerMask.GetMask(layers[i]);
            }
        }
        if (angle < viewAngle / 2 && angle > -viewAngle / 2 || dis <= viewRadius)
        {
            //碰撞！
            RaycastHit2D hit = Physics2D.Raycast(enemyPos, (playerPos - enemyPos).normalized, int.MaxValue, layersMask);
            if (hit.collider != null)
            {
                if ((hit.point - enemyPos).sqrMagnitude >= dis * 0.99)
                {
                    colliderPos = playerPos;
                    return true ;
                }
                else {
                    colliderPos = hit.point;
                    return false;
                }
            }
            colliderPos = playerPos;
            return true;
        }
        else
        {
            colliderPos = enemyPos;
            return false;
        }
    }
    bool CanRun()
    {
        if (rb == null) { return false; }
        float x = 1 * forward.x;
        int y = 1;
        Vector2 start = new Vector2(rb.position.x + x, rb.position.y + y);
        RaycastHit2D hit = Physics2D.Raycast(start, new Vector2(0, -1f), 3, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("OneWayPlatform"));
        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }
    float getAngle(Vector3 fromVector, Vector3 toVector)
    {
        float angle = Vector3.Angle(fromVector, toVector);
        Vector3 normal = Vector3.Cross(fromVector, toVector);
        angle *= Mathf.Sign(Vector3.Dot(normal, up));
        return angle;
    }
    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            if (state == EnemyState.PATROL)
            {
                if (isToEnd)
                {   // 2 的 n次 4就是第三个地图
                    seeker.StartPath(rb.position, new Vector3(patrolEnd.x, patrolEnd.y, -1), OnPathComplete, 1 << (graphNum - 1));
                }
                else
                {
                    seeker.StartPath(rb.position, new Vector3(patrolStart.x, patrolStart.y, -1), OnPathComplete, 1 << (graphNum - 1));
                }
            }
            else
            {
                seeker.StartPath(rb.position, target.position, OnPathComplete);
            }

        }


    }
    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            toEnd = false;
            currentWayPoint = 0;
        }
    }
    public static Vector2 RIGHTFORWARD = new Vector2(1f, 0f);
    public static Vector2 LEFTFORWARD = new Vector2(-1f, 0f);
    // Update is called once per frame
    void FixedUpdate()
    {
        float rangerSqr = ((Vector2)this.enemyGFX.position - (Vector2)target.position).sqrMagnitude;

        switch (state)
        {
            case EnemyState.PATROL:
                //距离小于视野
                if (rangerSqr <= viewRadius * viewRadius)
                {
                    //在视野里或者实在太近
                    if (isInSight(rangerSqr))
                    {
                        if (lastInSightToTrack > toTrackSecond)
                        {
                            lastStartTrack = 0f;
                            sightColor = Color.yellow;
                            state = EnemyState.TRACK;
                            lastInSight = 0f;
                        }
                        else
                        {
                            lastInSightToTrack += Time.deltaTime;
                            updateFireBar(lastInSightToTrack / toTrackSecond, Color.green, Color.yellow);
                        }

                    }
                }
                //
                if (track())
                {
                    isToEnd = !isToEnd;
                    toEnd = true;
                    UpdatePath();
                }
                break;
            case EnemyState.TRACK:
                track();

                //至少追踪五秒后，并且离开 视野 后停止最终
                if ((lastStartTrack > trackSecond && !isInSight(rangerSqr)))
                {
                    lastInSight = 0f;
                    lastInSightToTrack = toTrackSecond / 2;
                    sightColor = Color.green;
                    state = EnemyState.PATROL;
                    //巡逻模式停止开火
                    eneity.SendMessage("stopFire", "");
                    updateFireBar(lastInSightToTrack / toTrackSecond, Color.green, Color.yellow);
                }
                else if (rangerSqr <= viewRadius * viewRadius && isInSight(rangerSqr))
                {
                    if (!onlyTrack) {
                        if (lastInSight > toFireSecond)
                        {
                            state = EnemyState.FIRE;
                            sightColor = Color.red;
                            lastInSight = 0f;
                            nowFireSec = 0f;
                            eneity.SendMessage("startFire", "");
                        }
                        lastInSight += Time.deltaTime;
                        updateFireBar(lastInSight / toFireSecond, Color.yellow, Color.red);
                    }
                    
                }
                break;
            case EnemyState.FIRE:
                if ((nowFireSec >= fireSecond) &&(rangerSqr > viewRadius * viewRadius || !isInSight(rangerSqr)))
                {
                    lastStartTrack = 0f;
                    lastInSight = 0f;
                    sightColor = Color.yellow;
                    state = EnemyState.TRACK;
                    eneity.SendMessage("stopFire", "");
                }
                else
                {
                    StandBy();
                    nowFireSec += Time.deltaTime;
                    eneity.SendMessage("startFire", "");
                }
                break;
        }

        // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

        // apply gravity before moving
        _velocity.y += gravity * Time.deltaTime;

        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms
        if (_controller.isGrounded && Input.GetKey(KeyCode.S))
        {
            _velocity.y *= 3f;
            _controller.ignoreOneWayPlatformsThisFrame = true;
        }

        _controller.move(_velocity * Time.deltaTime);

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
    }
    private bool isInSight(float rangerSqr)
    {
        Vector2 end;
        return getSightPoint(rb.position, target.position, out end, "ground", "Default");
    }
    float lastStartTrack = 0;
    float lastInSight = 0;
    float lastInSightToTrack = 0; 
    float turnRound = 0;
    float nowFireSec = 0;
    bool track()
    {

        if (path == null)
        {
            return false;
        }

        else if (currentWayPoint >= path.vectorPath.Count)
        {
            if (toEnd)
            {
                currentWayPoint = path.vectorPath.Count - 1;
            }
            else
            {
                return true;
            }

        }



        //得到施加力的方向，记得加线性阻力，否则不会停
        if (currentWayPoint == 0) { currentWayPoint = 1; }
        Vector2 direction = ((Vector2)path.vectorPath[currentWayPoint] - rb.position).normalized;

        Vector2 force = direction * 1 * Time.deltaTime;
        lastStartTrack += Time.deltaTime;




        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);
        if (distance < nextWayPointDistance)
        {
            currentWayPoint++;
        }

        if (_controller.isGrounded)
        {
            _velocity.y = 0;
            //bool land = false;
            //if (anim.GetBool("inSky"))
            //{
            //    land = true;
            //}
            OnTheGround();
            //if (land)
            //{
            //    anim.SetBool("isLanding", true);
            //    //这里停顿0.5秒表示落地硬直
            //    anim.SetBool("isLanding", false);
            //}
            //Debug.Log("on the ground!");
        }
        else
        {
            InSky();
        }
        //向右-转身
        if (direction.x > 0f)
        {
            Running();
            normalizedHorizontalSpeed = 1;
            //如果当前是朝左-默认scale是朝左的，所以localScale>0就是原图像
            if (enemyGFX.localScale.x > 0f && turnRound >= turnRoundSecond)
            {
                enemyGFX.localScale = new Vector3(-Mathf.Abs(enemyGFX.localScale.x), enemyGFX.localScale.y, enemyGFX.localScale.z);
                turnRound = 0;
                forward = RIGHTFORWARD;
            }
            else if (turnRound < turnRoundSecond) {
                turnRound += Time.deltaTime;
                StandBy();
            }
        }
        // 向左-转身
        else if (direction.x < 0f)
        {
            Running();
            normalizedHorizontalSpeed = -1;
            if (enemyGFX.localScale.x < 0f && turnRound <= -turnRoundSecond)
            {
                enemyGFX.localScale = new Vector3(Mathf.Abs(enemyGFX.localScale.x), enemyGFX.localScale.y, enemyGFX.localScale.z);
                turnRound = 0;
                forward = LEFTFORWARD;
            }
            else if (turnRound > -turnRoundSecond)
            {
                turnRound -= Time.deltaTime;
                StandBy(); 
            }
            //if (_controller.isGrounded)
            //    _animator.Play(Animator.StringToHash("Run"));
        }
        else
        {
            StandBy();
            normalizedHorizontalSpeed = 0;

            //if (_controller.isGrounded)
            //    _animator.Play(Animator.StringToHash("Idle"));
        }
        if (!CanRun())
        {
            StandBy();
            return false;
        }
        return false;
    }


    // ---------------辅助函数------------------------
    private void Running()
    {
        isRunning = true;
        //anim.SetBool("isRunning", true);
    }
    private void StandBy()
    {
        isRunning = false;
        normalizedHorizontalSpeed = 0;
        //anim.SetBool("isRunning", false);
    }
    private void updateFireBar(float percent, Color from, Color to)
    {
        warningBar.material.color = Color.Lerp(from, to, percent);
        warningBar.transform.localScale = new Vector3(warningFireBarScale.x * (1 - percent), 1, 1);
    }
    private void InSky()
    {
        //anim.SetBool("inSky", true);
    }
    private void OnTheGround()
    {
        //anim.SetBool("inSky", false);
    }
}