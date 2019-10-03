using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BouncePlatform : MonoBehaviour
{
    //public enum Direction
    //{
    //    Up = 0,
    //    Left,
    //    Right
    //}

    //public Direction bounceDiection;
    public float jumpForce = 30;
    private Rigidbody2D rb2d;
    private PlayerCharacter pc;
    public float Threshold = 1f;
    public bool bounceReady = true;
    public float timer;
    // Start is called before the first frame update
    void Start()
    {
        timer = Threshold;
    }

    // Update is called once per frame
    void Update()
    {
        if (bounceReady == false)
        {
            timer -= Time.deltaTime;
        }

        if (timer < 0)
        {
            bounceReady = true;
            timer = Threshold;
        }

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag.CompareTo("Player") == 0)
        {
            if (bounceReady == true)
            {
                rb2d = col.gameObject.GetComponent<Rigidbody2D>();
                pc = col.gameObject.GetComponent<PlayerCharacter>();
                pc.GetFSM().CurrentStateIndex = 1;
                // kill any Y-axis speed

                //rb2d.velocity = gameObject.transform.up * jumpForce;
                rb2d.AddForce(gameObject.transform.up * jumpForce * 50);

                //switch (bounceDiection)
                //{
                //    case Direction.Up:
                //        rb2d.velocity = new Vector2(rb2d.velocity.x * 0.3f, jumpForce);
                //        break;
                //    case Direction.Left:
                //        rb2d.velocity = new Vector2(-jumpForce, rb2d.velocity.y * 0.3f);
                //        break;
                //    case Direction.Right:
                //        rb2d.velocity = new Vector2(jumpForce, rb2d.velocity.y * 0.3f);
                //        break;
                //}


                //rb2d.AddForce(Vector3.up * jumpForce * 100);
                bounceReady = false;
                Debug.Log("Perform Bounch Jump");
            }

        }

    }


    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    ContactPoint2D contact = collision.contacts[0];
    //    Vector2 rawNormal = collision.contacts[0].normal;
    //    Debug.Log("normal" + rawNormal);

    //    var Normal = new Vector2(-rawNormal.x, -rawNormal.y);
    //    if (collision.gameObject.tag.CompareTo("Hunch") == 0)
    //    {
    //        if (bounceReady == true)
    //        {
    //            rb2d = collision.gameObject.GetComponent<Rigidbody2D>();
    //            pc = collision.gameObject.GetComponent<PlayerCharacter>();
    //            pc.GetFSM().CurrentStateIndex = 1;
    //            // kill any Y-axis speed

    //            //rb2d.velocity = new Vector2(Normal.x * jumpForce, Normal.y * jumpForce);
    //            rb2d.AddForce(Normal * jumpForce * 45);

    //            //switch (bounceDiection)
    //            //{
    //            //    case Direction.Up:
    //            //        rb2d.velocity = new Vector2(Normal.x * jumpForce, Normal.y * jumpForce);
    //            //        //rb2d.AddForce(Normal * jumpForce * 45);
    //            //        break;
    //            //    case Direction.Left:
    //            //        rb2d.velocity = new Vector2(-jumpForce, rb2d.velocity.y * 0.3f);
    //            //        break;
    //            //    case Direction.Right:
    //            //        rb2d.velocity = new Vector2(jumpForce, rb2d.velocity.y * 0.3f);
    //            //        break;
    //            //}


    //            //rb2d.AddForce(Vector3.up * jumpForce * 100);
    //            bounceReady = false;
    //            Debug.Log("Perform Bounch Jump");
    //        }

    //    }
    //}
}