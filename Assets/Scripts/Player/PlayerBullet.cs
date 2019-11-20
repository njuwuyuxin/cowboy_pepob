using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public Bullettype BulletType;
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
        if(col.tag!="Player")
            Debug.Log(col.tag);
        if (col.tag == "Enemy")
        {
            col.gameObject.GetComponent<EnemyHealth>().Hurt(DamagePerShoot);
            if(BulletType==Bullettype.Blood)
            {
                PlayerManager._PlayerManager.PlayerHP += (int)(DamagePerShoot * 0.4f);
                if (PlayerManager._PlayerManager.PlayerHP > PlayerManager._PlayerManager.PlayerHPMax)
                    PlayerManager._PlayerManager.PlayerHP = PlayerManager._PlayerManager.PlayerHPMax;
                PlayerManager._PlayerManager.UpdateHPUI();
            }
            Destroy(gameObject);
        }
        else if (col.gameObject.tag != "Player"&& col.gameObject.tag != "Enemy_Bullet"&&col.gameObject.tag != "CallElevator")
        {
            Destroy(gameObject);
        }
    }
}
