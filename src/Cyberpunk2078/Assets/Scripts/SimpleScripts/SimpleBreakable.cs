﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

public class SimpleBreakable : MonoBehaviour
{
    private Animator anim;

    private bool triggered;

    private List<GameObject> frags;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        triggered = false;
        frags = new List<GameObject>();
        collectFrag();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyBreakable(Vector3 pos)
    {
        if (!triggered)
        {
            if(anim)anim.Play("Glass",0,-1);
            triggered = true;
            //Play break animation
            CameraManager.Instance.Shaking(0.1f,0.1f,true);
            GetComponent<BoxCollider2D>().enabled = false;
            explode(pos);
        }

    }
    
    private void Disable()
    {
        gameObject.SetActive(false);
        triggered = false;
        GetComponent<BoxCollider2D>().enabled = true;
    }


    private void collectFrag()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            frags.Add(transform.GetChild(i).gameObject);
        }

    }

    private void explode(Vector3 pos)
    {
        GetComponent<Explodable>().explode();
        foreach (var frag in frags)
        {
            float force = Random.Range(500, 1000);
            frag.GetComponent<Rigidbody2D>().AddForce(force * (frag.transform.position - pos).normalized);
            Destroy(frag,Random.Range(3f,8f));
        }
        
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.gameObject.tag == "PlayerHitBox")
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}

    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    if (other.gameObject.tag == "PlayerHitBox")
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}

}
