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
    // Start is called before the first frame update
    void Start()
    {
        ren = GetComponentInChildren<SpriteRenderer>();
        
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Death();
            transform.Translate(new Vector3(0,-sinkSpeed*Time.deltaTime));
        }
        else if(!dead&&currentHealth<startingHealth)
        {
            ren.sprite = damagedEnemy;
        }
    }
    public void Hurt(int amount)
    {
        currentHealth -= amount;
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
        Debug.Log("Death");
        // Allow the enemy to rotate and spin it by adding a torque.

        // Find all of the colliders on the gameobject and set them all to be triggers.
        Collider2D[] cols = GetComponents<Collider2D>();
        foreach (Collider2D c in cols)
        {
            c.isTrigger = true;
        }
        Destroy(gameObject, 1);
    }
}
