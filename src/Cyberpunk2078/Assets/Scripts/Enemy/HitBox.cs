using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class HitBox : MonoBehaviour
{
    public float damage = 1;
    public bool isFriendly = false;

    private HashSet<int> objectsHit = new HashSet<int>();


    private void Start()
    {
        objectsHit.Clear();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFriendly)
        {

        }
        else if (other.tag == "Player")
        {
            if (!objectsHit.Contains(other.gameObject.GetInstanceID()))
            {
                PlayerCharacter player = other.GetComponent<PlayerCharacter>();

                if (player.State.Name != "Dash")
                    objectsHit.Add(other.gameObject.GetInstanceID());


                other.GetComponent<PlayerCharacter>().ApplyDamage(GetInstanceID(), damage);
            }
        }
    }
}
