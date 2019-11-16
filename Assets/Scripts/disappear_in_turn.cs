using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disappear_in_turn : MonoBehaviour
{
    public GameObject[] bunkers;
    public float interval_time;
    bool is_enter = false;
    float timer = 0;
    int i = 0;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject i in bunkers)
        {
            i.SetActive(false);
            bunkers[0].SetActive(true);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(is_enter)
        {
     
                timer += Time.deltaTime;
                if (timer > interval_time)
                {
                    timer = 0;
                    bunkers[i].SetActive(false);
                    if(i==bunkers.Length-1)
                    {
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                    bunkers[i].SetActive(true);
                }
        }
            
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.tag=="Player")
        {
            is_enter = true;
            
        }
    }
    
}
