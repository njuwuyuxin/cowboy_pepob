﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class monster_trap : MonoBehaviour
{
    public GameObject monster;
    static public Vector2 respawn;
    static public Vector2 set_patrolStart;
    static public Vector2 set_patrolEnd;
    bool is_trigger = false;
    private void OnDrawGizmos()
    {
        Color color = Handles.color;
        Handles.color = Color.green;
        Handles.DrawSolidDisc(new Vector3(respawn.x, respawn.y, -1), new Vector3(0f, 0f, 1f), 0.3f);
        color = Handles.color;
        Handles.color = Color.yellow;
        Handles.DrawSolidDisc(new Vector3(set_patrolStart.x, set_patrolStart.y, -1), new Vector3(0f, 0f, 1f), 0.3f);
        Handles.DrawSolidDisc(new Vector3(set_patrolEnd.x, set_patrolEnd.y, -1), new Vector3(0f, 0f, 1f), 0.3f);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Player")
        {
            if(is_trigger==false)
            {
                Instantiate(monster);
                monster.transform.position = respawn;
                Debug.Log("respawn" + respawn);
                GroundEnemyAI GroundEnemyAI = monster.GetComponent<GroundEnemyAI>();
                if(GroundEnemyAI)
                {
                    GroundEnemyAI.patrolStart = set_patrolStart;
                    GroundEnemyAI.patrolEnd = set_patrolEnd;
                }
                

                is_trigger = true;
            }
            
        }
    }
}
