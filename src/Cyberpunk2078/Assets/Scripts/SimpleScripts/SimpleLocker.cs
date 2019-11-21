using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLocker : MonoBehaviour
{

    public SimpleLockedDoor ConnectedDoor;
    bool b = true;

    // Start is called before the first frame update
    void Awake()
    {
        ConnectedDoor.AddLocker(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (b && (other.gameObject.tag == "PlayerAttack" || other.gameObject.tag == "Player"))
        {
            ConnectedDoor.DeleteLocker();
            gameObject.SetActive(false);

            b = false;
        }
    }

}
