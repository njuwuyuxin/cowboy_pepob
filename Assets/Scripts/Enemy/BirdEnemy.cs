using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdEnemy : MonoBehaviour
{
    public float speed;
    public float stoppingDistance;

    private float timeBtwShots;
    public float startTimeBtwShots;

    public GameObject projectile;
    public Transform player;
    private bool canFire = false;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        timeBtwShots = startTimeBtwShots;
    }
    void startFire() {
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
        if (canFire) {
            if (timeBtwShots <= 0)
            {
                //发射子弹
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
