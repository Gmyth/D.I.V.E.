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
    public float jumpForce = 20;
    private Rigidbody2D rb2d;
    private PlayerCharacter pc;
    public float Threshold = 1f;
    public bool bounceReady = true;
    public float timer;
    public float angle;

    public bool isVertical = false;

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
            timer -= Time.unscaledDeltaTime;
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
                Player.CurrentPlayer.jumpForceGate = true;
                pc.GetFSM().CurrentStateIndex = 4;

                CameraManager.Instance.Shaking(0.1f,0.03f);
                CameraManager.Instance.Follow(0.3f);

                // kill any Y-axis speed
                rb2d.velocity = Vector2.zero;

                //Debug.Log("Direction:"+ gameObject.transform.up);
                //Debug.Log(LogUtility.MakeLogStringFormat("Bounce Platform","Force:" + gameObject.transform.up * jumpForce * 50 * 1 / Time.timeScale));

                var pos = col.gameObject.transform.position;
                //col.gameObject.transform.position = new Vector3(pos.x + gameObject.transform.up.x*0.2f, pos.y + gameObject.transform.up.y*0.2f, pos.z);
                rb2d.AddForce(gameObject.transform.up * jumpForce * 50 * 1/Time.timeScale);

                if(isVertical == true)
                {
                    rb2d.AddForce(-gameObject.transform.right * jumpForce * 50 * 1 / Time.timeScale);
                }
                
                Player.CurrentPlayer.AddNormalEnergy(1);

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
            }

        }

    }


    //void OnTriggerStay2D(Collider2D col)
    //{
    //    if (col.gameObject.tag.CompareTo("Player") == 0)
    //    {
    //        if (bounceReady == true)
    //        {
    //            rb2d = col.gameObject.GetComponent<Rigidbody2D>();
    //            pc = col.gameObject.GetComponent<PlayerCharacter>();

    //            pc.GetFSM().CurrentStateIndex = 4;
    //            Player.CurrentPlayer.jumpForceGate = true;
    //            CameraManager.Instance.Shaking(0.1f, 0.03f);
    //            CameraManager.Instance.Follow(0.3f);

    //            // kill any Y-axis speed
    //            rb2d.velocity = Vector2.zero;

    //            //Debug.Log("Direction:"+ gameObject.transform.up);
    //            //Debug.Log("Force:" + gameObject.transform.up * jumpForce * 50 * 1 / Time.timeScale);

    //            var pos = col.gameObject.transform.position;
    //            col.gameObject.transform.position = new Vector3(pos.x + gameObject.transform.up.x * 0.5f, pos.y + gameObject.transform.up.y * 0.5f, pos.z);
    //            rb2d.AddForce(gameObject.transform.up * jumpForce * 50 * 1 / Time.timeScale);

    //            Player.CurrentPlayer.AddNormalEnergy(1);

    //            //switch (bounceDiection)
    //            //{
    //            //    case Direction.Up:
    //            //        rb2d.velocity = new Vector2(rb2d.velocity.x * 0.3f, jumpForce);
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
    //        }

    //    }

    //}
}