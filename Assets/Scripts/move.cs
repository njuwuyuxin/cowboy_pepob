using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

//NORMAL指正常移动状态，HURTBACK指收到伤害被强制位移（无法控制）
enum MoveState { NORMAL,HURTBACK};

public class move : MonoBehaviour
{
    public float gravity = -25f;
    public float runSpeed = 8f;
    public float groundDamping = 20f; // how fast do we change direction? higher means faster
    public float inAirDamping = 5f;
    public float jumpHeight = 3f;
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

    //受到伤害并被击退
    public void hurtAndBack(int damage, float distance, float uncontrolableTime)
    {
        _playerManager.hurt(damage);
        MoveStatus = MoveState.HURTBACK;
        GetComponent<rope>().ResetRope();
        GetComponent<rope>().enabled = false;           //关闭钩索模块
        UncontrolableTime = uncontrolableTime;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        if(transform.localScale.x>0)        //面朝右
            _velocity.x = -distance;
        else                                              //面朝左
            _velocity.x=distance;
        _velocity.y = 3f;
        //_velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
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
        if (col.tag == "Spike")
        {
            Debug.Log("Enter spike");
            hurtAndBack(30, 15f,0.5f);
        }
    }

    void onTriggerStayEvent(Collider2D col)
    {

    }

    void onTriggerExitEvent(Collider2D col)
    {
        Debug.Log("onTriggerExitEvent: " + col.gameObject.name);
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
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

        if (_controller.isGrounded&&MoveStatus==MoveState.NORMAL)
        {
            _velocity.y = 0;
            //bool land = false;
            //if (anim.GetBool("inSky"))
            //{
            //    land = true;
            //}
            anim.SetBool("inSky", false);
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
            anim.SetBool("inSky", true);
        }

        if (Input.GetKey(KeyCode.D)&&MoveStatus==MoveState.NORMAL)
        {
            anim.SetBool("isRunning", true);
            normalizedHorizontalSpeed = 1;
            if (transform.localScale.x < 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            //if (_controller.isGrounded)
            //    _animator.Play(Animator.StringToHash("Run"));
        }
        else if (Input.GetKey(KeyCode.A) && MoveStatus == MoveState.NORMAL)
        {
            anim.SetBool("isRunning", true);
            normalizedHorizontalSpeed = -1;
            if (transform.localScale.x > 0f)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            //if (_controller.isGrounded)
            //    _animator.Play(Animator.StringToHash("Run"));
        }
        else
        {
            anim.SetBool("isRunning", false);
            normalizedHorizontalSpeed = 0;

            //if (_controller.isGrounded)
            //    _animator.Play(Animator.StringToHash("Idle"));
        }


        //we can only jump whilst grounded
        if (_controller.isGrounded && Input.GetKeyDown(KeyCode.Space) && MoveStatus == MoveState.NORMAL)
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
}
