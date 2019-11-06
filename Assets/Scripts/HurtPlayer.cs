using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtPlayer : MonoBehaviour
{
    public move move;
    int damage;
    int distance;
    int move_time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.tag=="player")
        {
            move.hurtAndBack(damage,distance,move_time);
        }
    }
}
