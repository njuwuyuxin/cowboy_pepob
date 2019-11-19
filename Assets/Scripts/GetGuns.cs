using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Guntype {Blood,Assult};
public class GetGuns : MonoBehaviour
{
    public Guntype GunType;
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
        Debug.Log("触碰枪支宝箱");
        PlayerManager manager = col.GetComponent<PlayerManager>();

        if (GunType == Guntype.Blood)
        {
            manager.Gun2Lock = true;
        }
        if (GunType == Guntype.Assult)
        {
            manager.Gun3Lock = true;
        }
    }
}
