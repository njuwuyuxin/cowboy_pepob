using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Bullettype { Pistol,Blood, Assult };

public class GetBullets : MonoBehaviour
{
    public Bullettype BulletType;
    public int BulletCount;     //宝箱中藏有的子弹数量
    public bool PartialRandom=false;      //开启后会在原有子弹数量上随机小幅度波动，但不会过分随机

    private bool used = false;
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
        if (col.tag != "Player")
            return;
        if (used)
            return;
        Debug.Log("触碰子弹宝箱");
        shoot shootScript = col.GetComponent<shoot>();
        Debug.Log(shootScript);

        int count = BulletCount;
        if (PartialRandom)
        {
            count += Random.Range(-3, 3);
        }
        if (BulletType == Bullettype.Pistol)
        {
            shootScript.Guns[0].bulletStore += count;
        }
        if (BulletType == Bullettype.Blood)
        {
            shootScript.Guns[1].bulletStore += count;
        }
        if (BulletType == Bullettype.Assult)
        {
            shootScript.Guns[2].bulletStore += count;
        }
        shootScript.UpdateBulletCountUI();
        used = true;
    }
}
