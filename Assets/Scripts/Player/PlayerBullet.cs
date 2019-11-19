using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public int DamagePerShoot;
    // Start is called before the first frame update
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.tag);
        if (col.tag == "Enemy")
        {
            col.gameObject.GetComponent<EnemyHealth>().Hurt(DamagePerShoot);
            Destroy(gameObject);
        }
        else if (col.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }
}
