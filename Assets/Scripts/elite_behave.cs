using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
public class elite_behave : MonoBehaviour
{
    private CharacterController2D _controller;
    private float timer=0f;
    private int round_number=3;
    private GameObject player;
    private float bullet_wait_timer=0f;
    float time_wait_1 = 0;
    float time_wait_3 = 0;
    int shoot_number = 0;
    int i = 0;
    float time_wait_2 = 0;
    public GameObject follow_bullet;
    public GameObject bullet;
    //public Animator _animator;   
    public float round_time;
    public float rush_speed;
    public float bullet_speed;
    public float bullet_wait_time;
    public float rush_time;
    // Start is called before the first frame update
    void Start()
    {
        //_animator = GetComponent<Animator>();
        _controller = GetComponent<CharacterController2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        
        
        
        if (timer > round_time)
        {

            if (i == 0)
            {
                time_wait_1 += Time.deltaTime;
                if (time_wait_1 > bullet_wait_time)
                {
                    if (shoot_number < 3)
                    {
                        Round1();
                        shoot_number++;
                        time_wait_1 = 0;
                    }
                    if (shoot_number == 3)
                    {
                        shoot_number = 0;
                        i++;
                        timer = 0;
                    }

                }
            }
            else if (i == 1)
            {
                time_wait_2 += Time.deltaTime;
                if (time_wait_2 > bullet_wait_time)
                {
                    if (shoot_number < 3)
                    {
                        round2();
                        shoot_number++;
                        time_wait_2 = 0;
                    }
                    if (shoot_number == 3)
                    {
                        shoot_number = 0;
                        timer = 0;
                        i++;
                    }

                }
            }
            else if (i == 2)
            {
                time_wait_3 += Time.deltaTime;
                if (time_wait_3<rush_time)
                {
                    Vector3 pos = player.transform.position - transform.position;
                    _controller.move(pos * rush_speed * Time.deltaTime);
                }
                else
                {
                    time_wait_3 = 0;
                    timer = 0;
                    i = 0;
                }
                
            }
        }

    }
    void Round1()
    {
        Vector3 pos = player.transform.position - transform.position;
        float angle = Vector2.Angle(new Vector2(pos.x, pos.y), new Vector2(1, 0));
        if (pos.y < 0)
            angle = -angle;
        angle = angle * Mathf.Deg2Rad;
        
        
            Rigidbody2D bulletInstance = Instantiate(bullet.GetComponent<Rigidbody2D>(), transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
            bulletInstance.velocity = new Vector2(bullet_speed * Mathf.Cos(angle), bullet_speed * Mathf.Sin(angle));
            
        
    }
    IEnumerator  bullet_shoot_round1(float angle)
    {
        //Rigidbody2D bulletInstance = Instantiate(bullet.GetComponent<Rigidbody2D>(), transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
        //bulletInstance.velocity = new Vector2(bullet_speed * Mathf.Cos(angle), bullet_speed * Mathf.Sin(angle));
        yield return new WaitForSeconds(bullet_wait_time);
    }
    void round2()
    {
       
        
            Rigidbody2D bulletInstance=Instantiate(follow_bullet.GetComponent<Rigidbody2D>(), transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
            
        
            


    }
    IEnumerator bullet_shoot_round2()
    {
        Instantiate(follow_bullet);
        yield return new WaitForSeconds(bullet_wait_time);

    }
    void round3()
    {
        
        Vector3 pos = player.transform.position - transform.position;
        _controller.move(pos*rush_speed * Time.deltaTime);
    }
}
