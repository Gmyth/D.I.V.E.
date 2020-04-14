using System.Collections;
using System.Collections.Generic;
using MyBox;
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
            GetComponent<Animator>().Play("HeartConsume",0,0);
            GetComponentInChildren<ParticleSystem>().Play();

        }
    }

    private void TriggerDestroy()
    {
        GetComponent<SpriteRenderer>().color = Color.gray;
        GetComponentInChildren<ParticleSystem>().Stop();
       //gameObject.SetActive(false);
    }
}
