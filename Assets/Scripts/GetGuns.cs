using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum Guntype {Pistol,Blood,Assult};
public class GetGuns : MonoBehaviour
{
    public Guntype GunType;
    public GameObject InteractiveUI;                //显示宝箱UI
    private bool used=false;
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
        if (used)
            return;
        if (col.tag != "Player")
            return;
        Debug.Log("触碰枪支宝箱");

        string UItext = "";
        PlayerManager manager = col.GetComponent<PlayerManager>();
        if (GunType == Guntype.Pistol)
        {
            manager.Gun1Lock = true;
            col.GetComponent<shoot>().GetFirstGun(0);
            UItext = "获得 手枪";
        }
        if (GunType == Guntype.Blood)
        {
            manager.Gun2Lock = true;
            col.GetComponent<shoot>().GetFirstGun(1);
            UItext = "获得 吸血枪";
        }
        if (GunType == Guntype.Assult)
        {
            manager.Gun3Lock = true;
            col.GetComponent<shoot>().GetFirstGun(2);
            UItext = "获得 突击步枪";
        }

        InteractiveUI.SetActive(true);
        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);
        Transform temp2 = InteractiveUI.transform.GetChild(0);
        Vector2 offset = new Vector2(10, 25);
        temp2.position = screenPos + offset;
        temp2.gameObject.GetComponent<Text>().text = UItext;
        Destroy(InteractiveUI, 2);

        used = true;
    }
}
