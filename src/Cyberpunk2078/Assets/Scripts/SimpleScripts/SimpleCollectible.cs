using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCollectible : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private int index;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Collect()
    {
        SimpleTutorialManager.Instance.PickUpCollectible(index);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Collect();
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Collect();
            gameObject.SetActive(false);
        }
    }

}
