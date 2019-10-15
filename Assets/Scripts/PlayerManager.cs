using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int PlayerHP = 100;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hurt(int damage)
    {
        PlayerHP -= damage;
        if (PlayerHP <= 0)
            Die();
    }

    void Die()
    {

    }
}
