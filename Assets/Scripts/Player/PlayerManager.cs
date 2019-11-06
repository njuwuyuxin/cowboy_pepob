using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HealthState { NORMAL,INVINCIBLE};
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager _PlayerManager = null; //单例模式

    private int PlayerHP;
    public int PlayerHPMax = 100;
    public float InvincibleTimeAfterDamage = 1f;                  //收到伤害后的无敌时间（指敌人碰撞，踩到陷阱)
    private float InvincibleTimer;                                               //受击无敌计时器
    private HealthState HealthStatus;
    private GameObject HPSlotUI;
    // Start is called before the first frame update
    void Awake()
    {
        if (_PlayerManager == null)
            _PlayerManager = this;
        else if (_PlayerManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        PlayerHP = PlayerHPMax;
        HPSlotUI = GameObject.Find("HP");
        HealthStatus = HealthState.NORMAL;
        InvincibleTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //该按键掉血仅供测试血量相关功能
        if (Input.GetKeyDown(KeyCode.Z))
        {
            hurt(10);
        }

        if (HealthStatus == HealthState.INVINCIBLE)
        {
            InvincibleTimer += Time.deltaTime;
        }
        if (HealthStatus == HealthState.INVINCIBLE && InvincibleTimer >= InvincibleTimeAfterDamage)
        {
            InvincibleTimer = 0;
            HealthStatus = HealthState.NORMAL;
        }
    }

    public void hurt(int damage)
    {
        if (HealthStatus == HealthState.NORMAL)
        {
            PlayerHP -= damage;
            HPSlotUI.GetComponent<Image>().fillAmount = (float)PlayerHP / (float)PlayerHPMax;
            if (PlayerHP <= 0)
                Die();
            HealthStatus = HealthState.INVINCIBLE;
        }
    }

    public HealthState GetHealthStatus()
    {
        return HealthStatus;
    }

    void Die()
    {
        PlayerHP = PlayerHPMax;
        HPSlotUI.GetComponent<Image>().fillAmount = (float)PlayerHP / (float)PlayerHPMax;
    }
}
