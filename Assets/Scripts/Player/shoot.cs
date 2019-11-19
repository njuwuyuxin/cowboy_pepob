using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum GunState {IDLE,SHOOTING,RELOADING,CHANGING};
public class GunInfo
{
    public int gunID;                                       //枪支编号
    public string gunName;                            //枪支名称
    public float shootingSpeed;                     //枪支射速
    public float flyingSpeed;                          //子弹飞行速度
    public int magVolume;                            //弹夹容量
    public int bulletLeft;                                //当前弹夹剩余子弹
    public int bulletStore;                              //子弹储备量
    public float reloadSpeed;                        //装弹时间（单位/秒）
    public GameObject Gun;                         //枪支的模型
    public GameObject Bullet;                      //子弹的模型
    public AudioClip shootSound;                //枪支发射时的音效

    public GunInfo(int gid, string gname, float shooting_speed, float flying_speed, int mag_volume,int bullet_store, float reload_speed, GameObject gun, GameObject bullet,AudioClip shoot_sound)
    {
        gunID = gid;
        gunName = gname;
        shootingSpeed = shooting_speed;
        flyingSpeed = flying_speed;
        magVolume = mag_volume;
        bulletLeft = magVolume;
        bulletStore = bullet_store;
        reloadSpeed = reload_speed;
        Gun = gun;
        Bullet = bullet;
        shootSound = shoot_sound;
    }
}
public class shoot : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeBetweenAttacks;
    public Rigidbody2D rocket;              // Prefab of the rocket.
    public float speed = 20f;               // The speed the rocket will fire at.
    public float ChangingTime = 0.3f;       //换枪所需时间
    private Animator anim;                  // Reference to the Animator component.
    private float shootingTimer;
    private float reloadingTimer;
    private float changingTimer;
    private GameObject BulletCountUI;
    //public AudioClip ShootSoundClip;

    private GunState GunStatus;
    public GameObject[] GunList;          //枪支实体列表,与子弹列表必须一一对应（没有枪支模型，暂时用空物体代替）
    public GameObject[] BulletList;       //子弹实体列表,与枪支列表必须一一对应
    public AudioClip[] ShootSounds;     //枪支发射音效列表
    public GunInfo[] Guns;                   //存储枪支列表信息的容器
    private GunInfo currentGun;           //目前手持的枪
    private Transform BulletLaunchPoint;           //主角身上的子物体，绑定在枪支头部，用来确定子弹起始位置
    private AudioSource shootSound;
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
        shootSound = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        GunStatus = GunState.IDLE;
        Guns = new GunInfo[3];
        Guns[0] = new GunInfo(
            1,                              //枪支编号
            "左轮枪",                  //枪支名称
            0.4f,                         //枪支射速
            20f,                          //子弹飞行速度
            6,                             //弹夹容量
            100,                        //子弹储备量
            1f,                            //装弹时间
            GunList[0],              //枪支的模型
            BulletList[0],           //子弹的模型
            ShootSounds[0]
            );
        Guns[1] = new GunInfo(
            2,                              //枪支编号
            "吸血枪",                  //枪支名称
            1f,                            //枪支射速
            15f,                          //子弹飞行速度
            5,                             //弹夹容量
            20,                           //子弹储备量
            1.5f,                         //装弹时间
            GunList[1],              //枪支的模型
            BulletList[1],           //子弹的模型
            ShootSounds[1]
            );
        Guns[2] = new GunInfo(
           3,                              //枪支编号
           "突击步枪",               //枪支名称
           0.1f,                          //枪支射速
           25f,                          //子弹飞行速度
           30,                           //弹夹容量
           120,                         //子弹储备量
           1.5f,                         //装弹时间
           GunList[2],              //枪支的模型
           BulletList[2],           //子弹的模型
           ShootSounds[2]
           );
        currentGun = Guns[0];
        shootingTimer =currentGun.shootingSpeed;           //初始设置为最大是为了保证射击第一枪可以无延迟射出
        reloadingTimer = 0;
        changingTimer = 0;
        BulletCountUI = GameObject.Find("BulletCount");
        UpdateBulletCountUI();
        BulletLaunchPoint = transform.GetChild(1).GetChild(0).GetChild(0).GetChild(3).GetChild(0).GetChild(0).GetChild(1);

        //shootableMask = LayerMask.GetMask("Shootable");
        //player = GameObject.FindGameObjectWithTag("Player");
        //anim = transform.root.gameObject.GetComponent<Animator>();
        //playerCtrl = transform.root.GetComponent<PlayerControl>();
    }

    public void UpdateBulletCountUI()
    {
        BulletCountUI.GetComponent<Text>().text = currentGun.bulletLeft.ToString() + "/"+currentGun.bulletStore.ToString();
    }
    private void shootEvent()
    {
        // ... set the animator Shoot trigger parameter and play the audioclip.
        //anim.SetTrigger("Shoot");
        //GetComponent<AudioSource>().Play();
        Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 MousePosition2D = new Vector2(MousePosition.x, MousePosition.y);
        //根据鼠标点击位置更改人物朝向
        if (MousePosition2D.x > transform.position.x && transform.localScale.x < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        if (MousePosition2D.x < transform.position.x && transform.localScale.x > 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        Vector3 pos = MousePosition - BulletLaunchPoint.position;
        float angle = Vector2.Angle(new Vector2(pos.x, pos.y), new Vector2(1, 0));
        if (pos.y < 0)
            angle = -angle;
        angle = angle * Mathf.Deg2Rad;
        
        Rigidbody2D bulletInstance = Instantiate(currentGun.Bullet.GetComponent<Rigidbody2D>(), BulletLaunchPoint.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
        bulletInstance.velocity = new Vector2(currentGun.flyingSpeed * Mathf.Cos(angle),currentGun.flyingSpeed * Mathf.Sin(angle));
        if (bulletInstance.velocity.x < 0)
            bulletInstance.transform.localScale = new Vector2(-bulletInstance.transform.localScale.x, bulletInstance.transform.localScale.y);

        if (currentGun.bulletLeft > 0)
           currentGun.bulletLeft--;
        else
            Debug.LogError("Err: 剩余弹药量小于0");

        shootSound.clip = currentGun.shootSound;
        shootSound.Play();
        UpdateBulletCountUI();
    }

    void Update()
    {
        //每帧判断鼠标位置决定人物朝向
        Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 MousePosition2D = new Vector2(MousePosition.x, MousePosition.y);
        if (MousePosition2D.x > transform.position.x && transform.localScale.x < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        if (MousePosition2D.x < transform.position.x && transform.localScale.x > 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        shootingTimer += Time.deltaTime;
        changingTimer += Time.deltaTime;
        if (GunStatus == GunState.CHANGING)     //切枪过程中
        {
            anim.SetBool("isSwitching", true);
            if (changingTimer < ChangingTime)
            {
                changingTimer += Time.deltaTime;
                return;
            }
            else
            {
                GunStatus = GunState.IDLE;      //切枪已完成
                //TODO: 动画相关设置
                anim.SetBool("isSwitching", false);
                UpdateBulletCountUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.R)&&GunStatus!=GunState.RELOADING)
        {
            if(currentGun.bulletLeft!=currentGun.magVolume&&currentGun.bulletStore>0)            //弹夹非满，且有弹药储备
            {
                GunStatus = GunState.RELOADING;
                UpdateBulletCountUI();
                Debug.Log("Reloading!");
            }
        }
        if (GunStatus == GunState.RELOADING)
        {
            anim.SetBool("isReloading", true);
            if (reloadingTimer <currentGun.reloadSpeed)
            {
                reloadingTimer += Time.deltaTime;
                return;
            }
            else
            {
                reloadingTimer = 0;
                int bulletNeed = currentGun.magVolume - currentGun.bulletLeft;
                if (currentGun.bulletStore >= bulletNeed)   //当前储备大于此次补充量
                {
                    currentGun.bulletStore -= bulletNeed;
                    currentGun.bulletLeft = currentGun.magVolume;
                }
                else
                {
                    currentGun.bulletLeft += currentGun.bulletStore;
                    currentGun.bulletStore = 0;
                }
                GunStatus = GunState.IDLE;
                anim.SetBool("isReloading", false);
                UpdateBulletCountUI();
                Debug.Log("Reloading Finished!");
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentGun = Guns[0];
            shootingTimer =currentGun.shootingSpeed;
            changingTimer = 0;
            GunStatus = GunState.CHANGING;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)&&GetComponent<PlayerManager>().Gun2Lock)
        {
            currentGun = Guns[1];
            shootingTimer =currentGun.shootingSpeed;
            changingTimer = 0;
            GunStatus = GunState.CHANGING;  
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && GetComponent<PlayerManager>().Gun3Lock)
        {
            currentGun = Guns[2];
            shootingTimer = currentGun.shootingSpeed;
            changingTimer = 0;
            GunStatus = GunState.CHANGING;
        }

        if (Input.GetMouseButtonDown(0))
        {
            GunStatus = GunState.SHOOTING;
            if (currentGun.gunID == 2) //吸血枪设置前摇
            {
                float shootAfterPress = 0.8f;       //前摇时间
                shootingTimer = currentGun.shootingSpeed - shootAfterPress;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            anim.SetBool("isShooting", false);
            GunStatus = GunState.IDLE;
        }
        if (Input.GetMouseButton(0))
        {

            if (shootingTimer>=currentGun.shootingSpeed&&currentGun.bulletLeft>0)     //一次射击事件
            {
                anim.SetBool("isShooting", true);
                shootingTimer = 0;
                shootEvent();
                if (currentGun.bulletLeft == 0&&currentGun.bulletStore>0)             //弹夹打光自动进入换弹状态（如果还有剩余备弹）
                {
                    GunStatus = GunState.RELOADING;
                    anim.SetBool("isShooting", false);
                    Debug.Log("Reloading!");
                }
            }      
        }
        
    }
}
