using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody2D rocket;              // Prefab of the rocket.
    public float speed = 20f;               // The speed the rocket will fire at.
    Ray shootRay = new Ray();                       // A ray from the gun end forwards.
    RaycastHit shootHit;                               // A raycast hit to get information about what was hit
    private PlayerControl playerCtrl;       // Reference to the PlayerControl script.
    private Animator anim;                  // Reference to the Animator component.
    int shootableMask;
    //LineRenderer gunLine;
    int range = 100;
    int smothing;
    GameObject player;
    void Awake()
    {
        // Setting up the references.
        shootableMask = LayerMask.GetMask("Shootable");
        player = GameObject.FindGameObjectWithTag("Player");

        //anim = transform.root.gameObject.GetComponent<Animator>();
        playerCtrl = transform.root.GetComponent<PlayerControl>();

    }


    void Update()
    {      
        if (Input.GetButtonDown("Fire1"))
        {

            // ... set the animator Shoot trigger parameter and play the audioclip.
            //anim.SetTrigger("Shoot");
            //GetComponent<AudioSource>().Play();
           
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float angle = Vector2.Angle(new Vector2(pos.x, pos.y), new Vector2(1, 0));
            if (pos.y < 0)
                angle = -angle;
            angle = angle * Mathf.Deg2Rad;
            
            Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
            bulletInstance.velocity = new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));

        }
    }
}
