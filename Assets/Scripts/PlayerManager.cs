using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private int PlayerHP;
    public int PlayerHPMax = 100;
    private GameObject HPSlotUI;
    // Start is called before the first frame update
    void Awake()
    {
        PlayerHP = PlayerHPMax;
        HPSlotUI = GameObject.Find("HP");
    }

    // Update is called once per frame
    void Update()
    {
        //该按键掉血仅供测试血量相关功能
        if (Input.GetKeyDown(KeyCode.Z))
        {
            hurt(10);
        }
    }

    public void hurt(int damage)
    {
        PlayerHP -= damage;
        HPSlotUI.GetComponent<Image>().fillAmount=(float)PlayerHP/(float)PlayerHPMax;
        if (PlayerHP <= 0)
            Die();
    }

    void Die()
    {
        PlayerHP = PlayerHPMax;
        HPSlotUI.GetComponent<Image>().fillAmount = (float)PlayerHP / (float)PlayerHPMax;
    }
}
