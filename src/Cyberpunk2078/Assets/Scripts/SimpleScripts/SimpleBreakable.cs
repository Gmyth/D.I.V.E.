using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBreakable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestoryBreakable()
    {
        //Play break animation
        gameObject.SetActive(false);
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
