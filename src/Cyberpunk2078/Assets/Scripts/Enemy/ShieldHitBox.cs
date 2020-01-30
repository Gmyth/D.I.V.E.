using UnityEngine;


public abstract class ShieldHitBox : HitBox
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (isFriendly)
        {
            if (other.tag == "EnemyHitBox")
                OnHitEnemyHitBox((Enemy)other.GetComponent<HitBox>().hit.source);
        }
        else if (other.tag == "PlayerHitBox")
            OnHitPlayerHitBox((PlayerCharacter)other.GetComponent<HitBox>().hit.source);
    }


    protected virtual void OnHitEnemyHitBox(Enemy enemy)
    {
    }

    protected virtual void OnHitPlayerHitBox(PlayerCharacter player)
    {
    }
}
