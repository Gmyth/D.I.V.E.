using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHealthPotion : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerCharacter.Singleton.Heal(1);
            gameObject.SetActive(false);
        }
    }

}
