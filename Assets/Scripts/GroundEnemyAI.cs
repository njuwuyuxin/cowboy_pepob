﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using UnityEditor;
using Prime31;

public class GroundEnemyAI : MonoBehaviour
{
    public Transform target;
    private EnemyState state = EnemyState.PATROL;
    public float nextWayPointDistance = 3f;
    public float trackSecond = 5f;
    public float fireSecond = 1f;
    public Transform enemyGFX;
    public GameObject eneity;
    public Vector2 patrolStart;
    public Vector2 patrolEnd;
    Path path;
    int currentWayPoint = 0;
    bool reachedEndOfPath = false;
    Seeker seeker;
    Rigidbody2D rb;

    public float gravity = -25f;
    public float runSpeed = 8f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster
    public float inAirDamping = 5f;
    public float jumpHeight = 3f;
    public Vector3 _velocity;

    [HideInInspector]
    private float normalizedHorizontalSpeed = 0;

    private CharacterController2D _controller;
    private Animator _animator;
    private RaycastHit2D _lastControllerColliderHit;
    public Animator anim;

    void Awake()
    {
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
        Debug.Log("onTriggerEnterEvent: " + col.gameObject.name);
    }


    void onTriggerExitEvent(Collider2D col)
    {
        Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
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
    private bool isToEnd = true;
    private bool toEnd = false;
    private bool isRunning = false;
    void OnDrawGizmos()
    {
        Color color = Handles.color;
        Handles.color = sightColor;
        int angle = viewAngle / 2;
        //绕z轴旋转半个
        Vector3 startLine = Quaternion.Euler(0, 0, -angle) * forward;
        Handles.DrawSolidArc(this.enemyGFX.position, up, new Vector3(startLine.x, startLine.y, 0f), viewAngle, viewRadius);
        Handles.color = Color.blue;
        Handles.DrawSolidDisc(new Vector3(patrolStart.x, patrolStart.y, -1), up, 0.3f);
        Handles.DrawSolidDisc(new Vector3(patrolEnd.x, patrolEnd.y, -1), up, 0.3f);
        if (state == EnemyState.PATROL || state == EnemyState.TRACK || state == EnemyState.FIRE)
        {
            if (rb == null) { return; }
            RaycastHit2D hit = Physics2D.Raycast(rb.position, ((Vector2)target.position - rb.position).normalized, int.MaxValue, 1 << LayerMask.NameToLayer("OneWayPlatform"));
            Vector3 end = target.position;
            if (hit.collider != null)
            {
                end = hit.point;
            }
            Handles.DrawBezier(rb.position, end, rb.position, end, Color.magenta, null, 3);
        }
        if (isRunning) {
            if (rb == null) { return; }
            float x = 1 * forward.x;
            int y = 1;
            Vector2 start = new Vector2(rb.position.x + x, rb.position.y + y);
            RaycastHit2D hit = Physics2D.Raycast(start, new Vector2(0,-1f), 8, 1 << LayerMask.NameToLayer("Default") | 1 << LayerMask.NameToLayer("OneWayPlatform"));
            Vector3 end = new Vector2(rb.position.x + x, rb.position.y + y-8);
            if (hit.collider != null)
            {
                end = hit.point;
            }
            Handles.DrawBezier(start, end, start, end, Color.magenta, null, 3);
        }

        Handles.color = color;

    }
    bool CanRun() {
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
                {
                    seeker.StartPath(rb.position, new Vector3(patrolEnd.x, patrolEnd.y, -1), OnPathComplete);
                }
                else
                {
                    seeker.StartPath(rb.position, new Vector3(patrolStart.x, patrolStart.y, -1), OnPathComplete);
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
                        lastStartTrack = 0f;
                        sightColor = Color.yellow;
                        state = EnemyState.TRACK;
                        //暂时追踪模式疯狂开火
                        lastInSight = 0f;
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
                    sightColor = Color.green;
                    state = EnemyState.PATROL;
                    //巡逻模式停止开火
                    eneity.SendMessage("stopFire", "");
                }
                else if (rangerSqr <= viewRadius * viewRadius && isInSight(rangerSqr))
                {

                    if (lastInSight > fireSecond)
                    {
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
                else
                {
                    StandBy();
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
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }
    float lastStartTrack = 0;
    float lastInSight = 0;
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
            else {
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
        //Debug.Log(rb.velocity.magnitude);
        //转身

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

        if (direction.x > 0f)
        {
            Running();
            normalizedHorizontalSpeed = 1;
            if (enemyGFX.localScale.x > 0f) {
                forward = RIGHTFORWARD;
                enemyGFX.localScale = new Vector3(-Mathf.Abs(enemyGFX.localScale.x), enemyGFX.localScale.y, enemyGFX.localScale.z);
                forward = RIGHTFORWARD;
            }
            //if (_controller.isGrounded)
            //    _animator.Play(Animator.StringToHash("Run"));
        }
        else if (direction.x < 0f)
        {
            Running();
            normalizedHorizontalSpeed = -1;
            if (enemyGFX.localScale.x < 0f) { 
                enemyGFX.localScale = new Vector3(Mathf.Abs(enemyGFX.localScale.x), enemyGFX.localScale.y, enemyGFX.localScale.z);
                forward = LEFTFORWARD;
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
    private void Running() {
        isRunning = true;
        //anim.SetBool("isRunning", true);
    }
    private void StandBy() { 
        isRunning=false;
        normalizedHorizontalSpeed = 0;
        //anim.SetBool("isRunning", false);
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