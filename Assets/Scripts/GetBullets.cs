using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Bullettype { Pistol,Blood, Assult };

public class GetBullets : MonoBehaviour
{
    public Bullettype BulletType;
    public int BulletCount;     //宝箱中藏有的子弹数量
    public bool PartialRandom=false;      //开启后会在原有子弹数量上随机小幅度波动，但不会过分随机
    public GameObject InteractiveUI;                //显示宝箱UI

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
        string UItext="";

        int count = BulletCount;
        if (PartialRandom)
        {
            count += Random.Range(-3, 3);
        }
        if (BulletType == Bullettype.Pistol)
        {
            shootScript.Guns[0].bulletStore += count;
            UItext = "获得手枪子弹x" + count;
        }
        if (BulletType == Bullettype.Blood)
        {
            shootScript.Guns[1].bulletStore += count;
            UItext = "获得吸血枪子弹x" + count;
        }
        if (BulletType == Bullettype.Assult)
        {
            shootScript.Guns[2].bulletStore += count;
            UItext = "获得突击步枪子弹x" + count;
        }
        shootScript.UpdateBulletCountUI();

        InteractiveUI.SetActive(true);
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Transform temp2 = InteractiveUI.transform.GetChild(0);
        Vector2 offset = new Vector2(60, 25);
        temp2.position = screenPos + offset;
        temp2.gameObject.GetComponent<Text>().text = UItext;
        Destroy(InteractiveUI, 2);

        used = true;
    }
}
