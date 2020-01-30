using UnityEngine;


public abstract class ShieldHitBox : HitBox
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (isFriendly)
        {
            if (other.tag == "EnemyHitBox")
                OnHitEnemyHitBox(other);
        }
        else if (other.tag == "PlayerHitBox")
            OnHitPlayerHitBox(other);
    }


    protected virtual void OnHitEnemyHitBox(Collider2D other)
    {
    }

    protected virtual void OnHitPlayerHitBox(Collider2D other)
    {
    }
}
