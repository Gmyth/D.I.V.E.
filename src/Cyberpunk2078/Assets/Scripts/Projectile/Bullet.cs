using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Bullet : Recyclable
{
    public bool isFriendly = false;
    public int numHits = 1;
    public int rawDamage = 100;

    private int numHitsRemaining;


    protected override void OnEnable()
    {
        base.OnEnable();

        numHitsRemaining = numHits;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFriendly)
        {
            if (other.tag == "Dummy")
            {
                other.GetComponent<PlayerCharacter>().ApplyDamage(rawDamage);
                Die();
            }
        }
        else if (other.tag == "Player")
        {
            if (other.GetComponent<PlayerCharacter>().State.Name != "Dash")
            {
                other.GetComponent<PlayerCharacter>().ApplyDamage(rawDamage);
            }
            Die();
        }
        
        else if (other.tag == "PlayerAttack")
        {
            
            Die();
        }
        
    }
}
