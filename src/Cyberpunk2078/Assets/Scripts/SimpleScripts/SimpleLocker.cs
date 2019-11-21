using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLocker : MonoBehaviour
{

    public SimpleLockedDoor ConnectedDoor;

    // Start is called before the first frame update
    void Start()
    {
        ConnectedDoor.AddLocker(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "PlayerAttack" || other.gameObject.tag == "Player")
        {
            ConnectedDoor.DeleteLocker();
            gameObject.SetActive(false);
        }
    }

}
