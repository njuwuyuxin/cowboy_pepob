using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOEnemy : MonoBehaviour
{
    // 暂时无用
    public float speed;
    // 暂时无用
    public float stoppingDistance;
    
    private float timeBtwShots;
    // 两次射击的间隔
    public float startTimeBtwShots;
    // 要发射的子弹
    public GameObject projectile;
    public Transform player;
    private bool canFire = false;
    // Start is called before the first frame update
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timeBtwShots = startTimeBtwShots;
    }
    void startFire()
    {
        canFire = true;
    }
    void stopFire()
    {
        canFire = false;
    }
    // Update is called once per frame
    void Update()
    {
        //if (Vector2.Distance(transform.position, player.position) > stoppingDistance) {

        //}
        if (canFire)
        {
            if (timeBtwShots <= 0)
            {
                //发射子弹
                anim.Play("shoot");
                Instantiate(projectile, transform.position, Quaternion.identity);
                timeBtwShots = startTimeBtwShots;
            }
            else
            {
                timeBtwShots -= Time.deltaTime;
            }
        }


    }
}
