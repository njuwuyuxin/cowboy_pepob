using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{        // The speed the enemy moves at.
    public int startingHealth = 100;            // The amount of health the enemy starts the game with.
    public int currentHealth;                  // How many times the enemy can be hit before it dies.
    public Sprite deadEnemy;            // A sprite of the enemy when it's dead.
    public Sprite damagedEnemy;
    private SpriteRenderer ren;
    private bool dead = false;
    public int sinkSpeed;
    private Animator anim;
    private float hurtTime = 0.2f;//敌人受击后变色时间
    private float hurtTimer;//计时器
    // Start is called before the first frame update
    void Start()
    {
        ren = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (ren.color == Color.red)
        {
            hurtTimer += Time.deltaTime;
            Debug.Log("hurtTimer is " + hurtTimer);
            if (hurtTimer >= hurtTime)
            {
                ren.color = Color.white;
                Debug.Log("white");
                hurtTimer = 0;
            }
        }
        if (currentHealth <= 0)
        {
            Death();
            transform.Translate(new Vector3(0,-sinkSpeed*Time.deltaTime));
        }
        else if(!dead&&currentHealth<startingHealth)
        {
            //ren.sprite = damagedEnemy;
        }
    }
    public void Hurt(int amount)
    {
        currentHealth -= amount;
        ren.color = Color.red;
        Debug.Log("red");
    }
    public void Die()
    {
        Death();
    }
    void onTriggerEnterEvent(Collider2D col)
    {
        Debug.Log("1");
        Debug.Log(col.tag);
        if(col.tag=="Spike")
        {
            Debug.Log("spike");
            Death();
        }
    }
    void onTriggerStayEvent(Collider2D col)
    {
        if (col.tag == "Spike")
        {
            Debug.Log("spike");
            Death();
        }
    }
    void Death()
    {
        // Find all of the sprite renderers on this object and it's children.
        SpriteRenderer[] otherRenderers = GetComponentsInChildren<SpriteRenderer>();

        // Disable all of them sprite renderers.
        foreach (SpriteRenderer s in otherRenderers)
        {
            s.enabled = false;
        }

        // Re-enable the main sprite renderer and set it's sprite to the deadEnemy sprite.
        

        // Set dead to true.
        dead = true;
        anim.Play("die");
        Debug.Log("Death");
        // Allow the enemy to rotate and spin it by adding a torque.

        // Find all of the colliders on the gameobject and set them all to be triggers.
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (Collider2D c in cols)
        {
            c.isTrigger = true;
        }

        //死亡时可以生成宝箱
        DropItem itemScript = GetComponent<DropItem>();
        if (itemScript != null)    //说明这个敌人身上存在掉落物品脚本
        {
            Vector3 pos = transform.position;
            if (itemScript.PositionType == PosType.Relative)
                pos += itemScript.pos;
            else
                pos = itemScript.pos;
            Instantiate(itemScript.item, pos, Quaternion.Euler(new Vector3(0, 0, 0)));
        }

        if(GetComponent<elite_behave>().UICanvas!=null)
        {
            GameObject temp = Instantiate(GetComponent<elite_behave>().UICanvas);
            Destroy(temp, 3);
        }

        Destroy(gameObject, 1);
    }
}
