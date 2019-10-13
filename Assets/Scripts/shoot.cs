using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeBetweenAttacks;
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
    float timer;
    GameObject player;
    void Awake()
    {
        // Setting up the references.
        shootableMask = LayerMask.GetMask("Shootable");
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        //anim = transform.root.gameObject.GetComponent<Animator>();
        playerCtrl = transform.root.GetComponent<PlayerControl>();
        timer = 0.2f;
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetBool("isShooting", true);
        }
        if (Input.GetMouseButtonUp(0))
        {
            anim.SetBool("isShooting", false);
        }
        if (Input.GetMouseButton(0))
        {
            timer += Time.deltaTime;
            if(timer>=timeBetweenAttacks)
            {

                // ... set the animator Shoot trigger parameter and play the audioclip.
                //anim.SetTrigger("Shoot");
                //GetComponent<AudioSource>().Play();
                timer = 0;
                Vector3 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 MousePosition2D = new Vector2(MousePosition.x, MousePosition.y);
                //根据鼠标点击位置更改人物朝向
                if (MousePosition2D.x > transform.position.x && transform.localScale.x < 0)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                if (MousePosition2D.x < transform.position.x && transform.localScale.x > 0)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

                Vector3 pos =MousePosition - transform.position;
                float angle = Vector2.Angle(new Vector2(pos.x, pos.y), new Vector2(1, 0));
                if (pos.y < 0)
                    angle = -angle;
                angle = angle * Mathf.Deg2Rad;
            
                Rigidbody2D bulletInstance = Instantiate(rocket, transform.position, Quaternion.Euler(new Vector3(0, 0, 0))) as Rigidbody2D;
                bulletInstance.velocity = new Vector2(speed * Mathf.Cos(angle), speed * Mathf.Sin(angle));

            }
            
        }
        else
        {
            timer = 0.2f;
        }
        
    }
}
