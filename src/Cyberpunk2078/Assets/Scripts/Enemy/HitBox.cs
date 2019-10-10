using System.Collections.Generic;
using UnityEngine;


public struct Hit
{
    public float damage;
    public float knockback;
}


[RequireComponent(typeof(Collider2D))]
public class HitBox : MonoBehaviour
{
    public Hit hit;
    public bool isFriendly = false;

    private Dummy dummy;
    private HashSet<int> objectsHit = new HashSet<int>();


    private void OnEnable()
    {
        dummy = GetComponentInParent<Dummy>();

        objectsHit.Clear();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFriendly)
        {

        }
        else if (other.tag == "Player")
        {
            PlayerCharacter target = other.GetComponent<PlayerCharacter>();

            if (target.State.Name != "Dash" && !objectsHit.Contains(other.gameObject.GetInstanceID()))
            {
                dummy.OnAttack?.Invoke();
                target.OnHit?.Invoke();


                if (hit.knockback > 0)
                    target.Knockback(dummy.transform.position, hit.knockback);


                target.ApplyDamage(GetInstanceID(), hit.damage);

                objectsHit.Add(other.gameObject.GetInstanceID());
            }
        }
    }
}
