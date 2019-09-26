using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
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
        
        if(timer < 0)
        {
            bounceReady = true;
            timer = Threshold;
        }
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag.CompareTo("Player") == 0)
        {
            if(bounceReady == true)
            {
                rb2d = col.gameObject.GetComponent<Rigidbody2D>();
                pc = col.gameObject.GetComponent<PlayerCharacter>();
                pc.GetFSM().CurrentStateIndex = 1;
                // kill any Y-axis speed
                rb2d.velocity = new Vector2(rb2d.velocity.x * 0.3f, jumpForce);
                //rb2d.AddForce(Vector3.up * jumpForce * 100);
                bounceReady = false;
                Debug.Log("Perform Bounch Jump");
            }

        }
        
    }


}
