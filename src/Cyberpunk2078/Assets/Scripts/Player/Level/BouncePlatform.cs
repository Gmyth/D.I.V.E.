﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BouncePlatform : MonoBehaviour
{
    public float jumpForce;
    private Rigidbody2D rb2d;
    private PlayerCharacter pc;
    public float Threshold;
    public bool bounceReady = true;
    private float timer;
    [SerializeField] private GameObject BounceVFX;
    
    [Range(0, 360)]
    [SerializeField]
    private float angle = 90f;

    Vector2 bounceDirection;

    // Start is called before the first frame update
    void Start()
    {
        timer = Threshold;
        BounceVFX.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bounceDirection = DegreeToVector2(angle);
        Debug.DrawRay(gameObject.transform.position, bounceDirection * jumpForce / 10, Color.red);

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

    void OnDrawGizmosSelected()
    {
        bounceDirection = DegreeToVector2(angle);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(bounceDirection.x, bounceDirection.y, 0) * jumpForce / 10);
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
                Player.CurrentPlayer.NoApplyFriction = true;
                pc.GetFSM().CurrentStateIndex = 4;

                CameraManager.Instance.Shaking(0.1f,0.03f);
                CameraManager.Instance.Follow(0.3f);

                // kill any Y-axis speed
                rb2d.velocity = Vector2.zero;
                rb2d.gravityScale = 3;
                //Debug.Log("Direction:"+ gameObject.transform.up);
                //Debug.Log(LogUtility.MakeLogStringFormat("Bounce Platform","Force:" + gameObject.transform.up * jumpForce * 50 * 1 / Time.timeScale));

                var pos = col.gameObject.transform.position;

                //col.gameObject.transform.position = new Vector3(pos.x + vec.x * 0.2f, pos.y + vec.y * 0.2f, pos.z);

                BounceVFX.SetActive(true);
                BounceVFX.GetComponent<Animator>().Play("BounceVFX", 0, 0);

                rb2d.AddForce(bounceDirection * jumpForce * 50 * 1/Time.timeScale);
              

                PlayerCharacter.Singleton.AddNormalEnergy(1);

                //rb2d.AddForce(Vector3.up * jumpForce * 100);
                bounceReady = false;
            }

        }

    }

    public static Vector2 RadianToVector2(float radian)
    {
        return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
    }

    public static Vector2 DegreeToVector2(float degree)
    {
        return RadianToVector2(degree * Mathf.Deg2Rad);
    }

}