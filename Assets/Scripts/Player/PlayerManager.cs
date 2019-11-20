using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HealthState { NORMAL,INVINCIBLE};
public class PlayerManager : MonoBehaviour
{
    public static PlayerManager _PlayerManager = null; //单例模式
    public bool RopeLock = false;           //判断主角是否获得绳索
    public bool Gun1Lock = false;           //判断主角是否获得第一把枪（手枪）
    public bool Gun2Lock = false;           //判断主角是否获得第二把枪（吸血枪）
    public bool Gun3Lock = false;           //判断主角是否获得第三把枪（突击步枪）

    public int PlayerHP;
    public int PlayerHPMax = 100;
    public float InvincibleTimeAfterDamage = 1f;                  //收到伤害后的无敌时间（指敌人碰撞，踩到陷阱)
    private float InvincibleTimer;                                               //受击无敌计时器
    private HealthState HealthStatus;
    private GameObject HPSlotUI;
    private Animator anim;
    // Start is called before the first frame update
    void Awake()
    {
        anim = GetComponent<Animator>();
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

        GetComponent<rope>().enabled = false;
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
            if (PlayerHP <= 0)
            {
                PlayerHP = 0;
                Die();
            }
            UpdateHPUI();
            HealthStatus = HealthState.INVINCIBLE;
        }
    }

    public HealthState GetHealthStatus()
    {
        return HealthStatus;
    }

    void Die()
    {
        anim.SetBool("isDied", true);
        PlayerHP = PlayerHPMax;
        HPSlotUI.GetComponent<Image>().fillAmount = (float)PlayerHP / (float)PlayerHPMax;
        GameManager._GameManager.GameOver();
        anim.SetBool("isDied", false);
    }

    public void UpdateHPUI()
    {
        HPSlotUI.GetComponent<Image>().fillAmount = (float)PlayerHP / (float)PlayerHPMax;
    }
}
