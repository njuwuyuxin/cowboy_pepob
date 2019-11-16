using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRopeAbility : MonoBehaviour
{
    public GameObject InteractiveUI;

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
        Debug.Log("触碰宝箱");
        rope PlayerRopeScript = col.GetComponent<rope>();
        if (PlayerRopeScript.enabled == false)
        {
            PlayerRopeScript.enabled = true;
            PlayerRopeScript.ResetRope();
            InteractiveUI.SetActive(true);
            Destroy(InteractiveUI, 3f);
        }
    }
}
