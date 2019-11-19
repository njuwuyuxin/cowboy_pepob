using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRocket : MonoBehaviour
{
    public float speed;
    private Transform player;
    private Vector2 target;
    private float timer;
    public float Follow_time;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = new Vector2(player.position.x, player.position.y);
        Vector3 v = transform.position - player.position;
        v.z = 0;
        Quaternion rotation = Quaternion.FromToRotation(Vector3.left, v);
        transform.rotation = rotation;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer< Follow_time)
        {
            Vector2 target = new Vector2(player.position.x, player.position.y);
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (transform.position.x == target.x && transform.position.y == target.y)
            {
                DestoryProjectile();
                Debug.Log("Hit Player");
                PlayerManager._PlayerManager.hurt(10);
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (transform.position.x == target.x && transform.position.y == target.y)
            {
                DestoryProjectile();
                Debug.Log("Hit Player");
                PlayerManager._PlayerManager.hurt(10);

            }
        }
        
    }

    void DestoryProjectile()
    {
        Destroy(gameObject);
    }

    void onTriggerEnter2D(Collider2D col)
    {
    }
}
