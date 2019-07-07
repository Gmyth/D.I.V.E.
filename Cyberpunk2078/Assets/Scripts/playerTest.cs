using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal.Input;

public class playerTest : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb2d;
    private float maxSpeed = 6;
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        if (Mathf.Abs(rb2d.velocity.y) < maxSpeed || rb2d.velocity.y * v <0)
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x , 0);
            rb2d.AddForce(v * transform.up *600);
        }
        
        if (Mathf.Abs(rb2d.velocity.x) < maxSpeed||rb2d.velocity.x * h <0)
        {
            rb2d.velocity = new Vector2(0,rb2d.velocity.y);
            rb2d.AddForce(h * transform.right *500);
        }

        if (Mathf.Abs(h) < 0.5 && Mathf.Abs(v) < 0.5)
        {
            rb2d.velocity = new Vector2(0,0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Dummy")
        {
            Vector2 direction = other.transform.position - transform.position;
            direction = direction.normalized;
            CameraManager.Instance.addTempTarget(other.gameObject, 0.5f);
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(direction *2000);

        }
    }
}
