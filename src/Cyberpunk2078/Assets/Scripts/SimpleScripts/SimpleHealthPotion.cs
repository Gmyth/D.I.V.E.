using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleHealthPotion : MonoBehaviour
{

    public bool isUsed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !isUsed)
        {
            isUsed = true;
            PlayerCharacter.Singleton.Heal(1);

            gameObject.SetActive(false);
        }
    }

}
