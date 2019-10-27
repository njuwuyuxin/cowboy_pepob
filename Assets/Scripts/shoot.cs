using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum GunState {IDLE,SHOOTING,RELOADING,CHANGING};
public class GunInfo
{
    public int gunID;                                       //枪支编号
    public string gunName;                            //枪支名称
    public float shootingSpeed;                     //枪支射速
    public float flyingSpeed;                          //子弹飞行速度
    public int magVolume;                            //弹夹容量
    public int bulletLeft;                                //当前弹夹剩余子弹
    public float reloadSpeed;                        //装弹时间
    public GameObject Gun;                         //枪支的模型
    public GameObject Bullet;                      //子弹的模型

    public GunInfo(int gid, string gname, float shooting_speed, float flying_speed, int mag_volume, float reload_speed, GameObject gun, GameObject bullet)
    {
        gunID = gid;
        gunName = gname;
        shootingSpeed = shooting_speed;
        flyingSpeed = flying_speed;
        magVolume = mag_volume;
        bulletLeft = magVolume;
        reloadSpeed = reload_speed;
        Gun = gun;
        Bullet = bullet;
    }
}
public class shoot : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeBetweenAttacks;
    public Rigidbody2D rocket;              // Prefab of the rocket.
    public float speed = 20f;               // The speed the rocket will fire at.
    private Animator anim;                  // Reference to the Animator component.
    private float timer;

    private GunState GunStatus;
    public GameObject[] GunList;          //枪支实体列表,与子弹列表必须一一对应（没有枪支模型，暂时用空物体代替）
    public GameObject[] BulletList;       //子弹实体列表,与枪支列表必须一一对应
    private GunInfo[] Guns;                   //存储枪支列表信息的容器
    private int GunIndex=0;                       //指明当前枪支索引，等于当前枪支编号-1
   
    //private int shootableMask;
    //LineRenderer gunLine;
    //int range = 100;
    //int smothing;
    //Ray shootRay = new Ray();                       // A ray from the gun end forwards.
    //RaycastHit shootHit;                               // A raycast hit to get information about what was hit
    //private PlayerControl playerCtrl;       // Reference to the PlayerControl script.
    //GameObject player;
    void Awake()
    {
        anim = GetComponent<Animator>();
        timer = 0.2f;

        GunIndex = 0;
        GunStatus = GunState.IDLE;
        Guns = new GunInfo[2];
        Guns[0] = new GunInfo(
            1,                              //枪支编号
            "左轮枪",                  //枪支名称
            0.2f,                         //枪支射速
            20f,                          //子弹飞行速度
            6,                             //弹夹容量
            1f,                            //装弹时间
            GunList[0],              //枪支的模型
            BulletList[0]            //子弹的模型
            );
        Guns[0] = new GunInfo(
            2,                              //枪支编号
            "时缓枪",                  //枪支名称
            1f,                         //枪支射速
            15f,                          //子弹飞行速度
            5,                             //弹夹容量
            1.5f,                            //装弹时间
            GunList[1],              //枪支的模型
            BulletList[1]            //子弹的模型
            );

        //shootableMask = LayerMask.GetMask("Shootable");
        //player = GameObject.FindGameObjectWithTag("Player");
        //anim = transform.root.gameObject.GetComponent<Animator>();
        //playerCtrl = transform.root.GetComponent<PlayerControl>();

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //TODO: 换弹
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GunIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GunIndex = 1;
        }

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("isShooting", true);
            GunStatus = GunState.SHOOTING;
        }
        if (Input.GetMouseButtonUp(0))
        {
            anim.SetBool("isShooting", false);
            GunStatus = GunState.IDLE;
        }
        if (Input.GetMouseButton(0))
        {
            timer += Time.deltaTime;
            if(timer>=Guns[GunIndex].shootingSpeed)
            {
                // ... set the animator Shoot trigger parameter and play the audioclip.
                //anim.SetTrigger("Shoot");
                //GetComponent<AudioSource>().Play();
                timer = 0;
                Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 MousePosition2D = new Vector2(MousePosition.x, MousePosition.y);
                //根据鼠标点击位置更改人物朝向
                if (MousePosition2D.x > transform.position.x && transform.localScale.x < 0)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                if (MousePosition2D.x < transform.position.x && transform.localScale.x > 0)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

                Vector3 pos =MousePosition - transform.position;
                float angle = Vector2.Angle(new Vector2(pos.x, pos.y), new Vector2(1, 0));
                if (pos.y < 0)
                    angle = -angle;
                angle = angle * Mathf.Deg2Rad;
            
                Rigidbody2D bulletInstance = Instantiate(Guns[GunIndex].Bullet.GetComponent<Rigidbody2D>(), transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                bulletInstance.velocity = new Vector2(Guns[GunIndex].flyingSpeed * Mathf.Cos(angle), Guns[GunIndex].flyingSpeed * Mathf.Sin(angle));
            }      
        }
        else
        {
            timer = Guns[GunIndex].shootingSpeed;
        }
        
    }
}
