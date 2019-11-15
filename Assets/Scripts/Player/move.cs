using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

//NORMAL指正常移动状态，HURTBACK指收到伤害被强制位移（无法控制）
enum MoveState { NORMAL,HURTBACK,USING_ELEVATOR,SQUATTING,LANDING}; //正常，受伤击退，使用电梯，下蹲，落地硬硬直

public class move : MonoBehaviour
{
    public float gravity = -25f;
    public float runSpeed = 8f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster
    public float inAirDamping = 5f;
    public float jumpHeight = 3f;
    public float LandingTime = 0.3f;    //落地硬直时间
    public Vector3 _velocity;

    [HideInInspector]
    private float normalizedHorizontalSpeed = 0;

    private CharacterController2D _controller;
    private PlayerManager _playerManager;
    private Animator _animator;
    private RaycastHit2D _lastControllerColliderHit;
    public Animator anim;
    private MoveState MoveStatus;
    private float UncontrolableTime;       //受到强制位移无法控制的时间
    private float UncontrolableTimer;     //强制位移计时器
    private float LandingTimer;               //落地硬直计时器



    //受到伤害并被击退
    public void hurtAndBack(int damage, float distance, float uncontrolableTime)
    {
        if (_playerManager.GetHealthStatus() == HealthState.NORMAL)
        {
            _playerManager.hurt(damage);
            MoveStatus = MoveState.HURTBACK;
            GetComponent<rope>().ResetRope();
            GetComponent<rope>().enabled = false;           //关闭钩索模块
            UncontrolableTime = uncontrolableTime;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            if (transform.localScale.x > 0)        //面朝右
                _velocity.x = -distance;
            else                                              //面朝左
                _velocity.x = distance;
            _velocity.y = 3f;
            //_velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
        }
    }

    //提供给电梯调用，用来设置主角移动状态，state = 0为USING_ELEVATOR，state = 1为NORMAL
    public void SetMoveStatus(int state)
    {
        if (state == 0)
            MoveStatus = MoveState.NORMAL;
        if (state == 1)
            MoveStatus = MoveState.USING_ELEVATOR;
    }

    #region Event Listeners

    void onControllerCollider(RaycastHit2D hit)
    {
        //Debug.Log("onControllerCollider = "+hit.collider.tag);
       GameObject colObject = hit.collider.gameObject;
        if (colObject.tag == "Elevator")
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                colObject.GetComponent<Elevator>().ElevatorStart();       //由于电梯移动时需要主角跟随移动，因此把主角物体作为参数传递
            }
        }

        // bail out on plain old ground hits cause they arent very interesting
        if (hit.normal.y == 1f)
            return;

        // logs any collider hits if uncommented. it gets noisy so it is commented out for the demo
        //Debug.Log( "flags: " + _controller.collisionState + ", hit.normal: " + hit.normal );
    }


    void onTriggerEnterEvent(Collider2D col)
    {
        //Debug.Log("onTriggerEnterEvent: " + col.gameObject.name);
        if (col.tag == "Spike"||col.tag=="Enemy")
        {
            hurtAndBack(30, 15f,0.5f);
        }
    }

    void onTriggerStayEvent(Collider2D col)
    {
        if (col.tag == "Spike" || col.tag == "Enemy")
        {
            hurtAndBack(30, 15f, 0.5f);
        }
    }

    void onTriggerExitEvent(Collider2D col)
    {
        //Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
    }

    #endregion

    #region MonoBehavior
    void Awake()
    {
        anim = GetComponent<Animator>();
        _controller = GetComponent<CharacterController2D>();
        _playerManager = GetComponent<PlayerManager>();

        // listen to some events for illustration purposes
        _controller.onControllerCollidedEvent += onControllerCollider;
        _controller.onTriggerEnterEvent += onTriggerEnterEvent;
        _controller.onTriggerStayEvent += onTriggerStayEvent;
        _controller.onTriggerExitEvent += onTriggerExitEvent;

        MoveStatus = MoveState.NORMAL;
        UncontrolableTime = 0;
        UncontrolableTimer = 0;
    }
    // Update is called once per frame
    void Update()
    {
        if (MoveStatus == MoveState.HURTBACK)
        {
            UncontrolableTimer += Time.deltaTime;
            if (UncontrolableTimer >= UncontrolableTime)
            {
                UncontrolableTime = 0;
                UncontrolableTimer = 0;
                MoveStatus = MoveState.NORMAL;
                GetComponent<rope>().enabled = true;
                GetComponent<rope>().ResetRope();
            }
        }


        Debug.Log(_velocity);
        LandingTimer += Time.deltaTime;
        if (_controller.isGrounded&&MoveStatus==MoveState.NORMAL)
        {
            if(_velocity.y<-7f) //从高空掉落，触发落地硬直
            {
                Debug.Log("触发落地硬直");
                LandingTimer = 0;
                MoveStatus = MoveState.LANDING;
                anim.SetBool("isLanding", true);
            }
            _velocity.y = 0;

            anim.SetBool("inSky", false);

            Debug.Log("on the ground!");
        }
        else
        {
            anim.SetBool("inSky", true);
        }
        if(MoveStatus==MoveState.LANDING)
        {
            if (LandingTimer < LandingTime)
                return;
            else
            {
                MoveStatus = MoveState.NORMAL;  //硬直结束，恢复正常
                anim.SetBool("isLanding", false);
            }
        }


        if (Input.GetKeyDown(KeyCode.S) && MoveStatus == MoveState.NORMAL && _controller.isGrounded)
        {
            MoveStatus = MoveState.SQUATTING;
            anim.SetBool("isSquatting", true);
        }
        if(Input.GetKeyUp(KeyCode.S)&&MoveStatus==MoveState.SQUATTING)
        {
            MoveStatus = MoveState.NORMAL;
            anim.SetBool("isSquatting", false);
        }
        if(MoveStatus==MoveState.SQUATTING)
        {
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            Debug.Log(col.size);
            col.size = new Vector2(1.12355f,0.65f); //该值的y值为下蹲时人物碰撞盒高度，该值的修改需要重新计算所有偏移量，否则会出现浮空或穿透现象
            col.offset = new Vector2(0.1019477f, 0.325f);
        }
        else
        {
            BoxCollider2D col = GetComponent<BoxCollider2D>();
            col.size = new Vector2(1.12355f, 1.330366f);
            col.offset = new Vector2(0.1019477f, 0.6514457f);
            //GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<BoxCollider2D>().size.x, GetComponent<BoxCollider2D>().size.y * 2);
        }

        if (Input.GetKey(KeyCode.D)&& (MoveStatus == MoveState.NORMAL || MoveStatus == MoveState.USING_ELEVATOR))
        {
            anim.SetBool("isRunning", true);
            normalizedHorizontalSpeed = 1;
            //if (transform.localScale.x < 0f)
            //    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (Input.GetKey(KeyCode.A) && (MoveStatus == MoveState.NORMAL || MoveStatus == MoveState.USING_ELEVATOR)) 
        { 
            anim.SetBool("isRunning", true);
            normalizedHorizontalSpeed = -1;
            //if (transform.localScale.x > 0f)
            //    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            anim.SetBool("isRunning", false);
            normalizedHorizontalSpeed = 0;

            //if (_controller.isGrounded)
            //    _animator.Play(Animator.StringToHash("Idle"));
        }


        //we can only jump whilst grounded
        if (((_controller.isGrounded && MoveStatus == MoveState.NORMAL)||MoveStatus==MoveState.USING_ELEVATOR) && Input.GetKeyDown(KeyCode.Space))
        {
            _velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            //_animator.Play(Animator.StringToHash("Jump"));
        }


        // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
        var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
        _velocity.x = Mathf.Lerp(_velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

        // apply gravity before moving
        _velocity.y += gravity * Time.deltaTime;

        // if holding down bump up our movement amount and turn off one way platform detection for a frame.
        // this lets us jump down through one way platforms
        if (_controller.isGrounded && Input.GetKey(KeyCode.S) && MoveStatus == MoveState.NORMAL)
        {
            _velocity.y *= 3f;
            _controller.ignoreOneWayPlatformsThisFrame = true;
        }

        _controller.move(_velocity * Time.deltaTime);

        // grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
    }
    #endregion
}

