using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEngine;

public class SimpleBreakable : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyBreakable()
    {
        anim.Play("Glass",0,-1);
        //Play break animation
        CameraManager.Instance.Shaking(0.1f,0.1f,true);
        GetComponent<BoxCollider2D>().enabled = false;
        
        
    }
    
    private void Disable()
    {
        gameObject.SetActive(false);
        GetComponent<BoxCollider2D>().enabled = true;
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
