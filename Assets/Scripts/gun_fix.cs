using UnityEngine;
using System.Collections;
public class gun_fix : MonoBehaviour
{
    public Rigidbody2D rocket;              // Prefab of the rocket.
    public float speed = 20f;               // The speed the rocket will fire at.
    Ray shootRay = new Ray();                       // A ray from the gun end forwards.
    RaycastHit shootHit;                               // A raycast hit to get information about what was hit
    private PlayerControl playerCtrl;       // Reference to the PlayerControl script.
    private Animator anim;                  // Reference to the Animator component.
    int shootableMask;
    LineRenderer gunLine;
    int range = 100;
    int smothing;
    GameObject player;
    void Awake()
    {
        // Setting up the references.
        shootableMask = LayerMask.GetMask("Shootable");
        player= GameObject.FindGameObjectWithTag("Player");
        gunLine = GetComponent<LineRenderer>();
        //anim = transform.root.gameObject.GetComponent<Animator>();
        playerCtrl = transform.root.GetComponent<PlayerControl>();
        
    }


    void Update()
    {
        Vector3 start = new Vector3(player.transform.position.x, player.transform.position.y, 0);
        Vector3 end = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        gunLine.SetPosition(0, start);
        gunLine.SetPosition(1, end);
        // If the fire button is pressed...
       
        if (Input.GetButtonDown("Fire1"))
        {
            
            // ... set the animator Shoot trigger parameter and play the audioclip.
            //anim.SetTrigger("Shoot");
            //GetComponent<AudioSource>().Play();
            //Vector3 start = Camera.main.ScreenToWorldPoint(new Vector3(player.transform.position.x, player.transform.position.y,0));
            
            //Vector3 end = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,0));
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float angle = Vector2.Angle(new Vector2(pos.x,pos.y), new Vector2(1, 0));
            start.z = 0;
            end.z = 0;
            if (pos.y < 0)
                angle = -angle;
            angle = angle  *Mathf.Deg2Rad;
            //shootRay.origin = transform.position;
            //shootRay.direction = pos;
            //Physics.Raycast(shootRay, out shootHit, range, shootableMask);
            //gunLine.SetPosition(0, start);
            //gunLine.SetPosition(1, end);
            // If the player is facing right...
           
                // ... instantiate the rocket facing right and set it's velocity to the right. 
                Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                bulletInstance.velocity = new Vector2(speed*Mathf.Cos(angle), speed*Mathf.Sin(angle));
           
        }
    }
}
