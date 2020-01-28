using UnityEngine;


public class ShieldHitBox : HitBox
{
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (isFriendly)
        {
        }
        else if (other.tag == "PlayerHitBox")
        {
            PlayerCharacter player = (PlayerCharacter)other.GetComponent<HitBox>().hit.source;


            hit.source.OnAttack.Invoke();
            player.OnHit?.Invoke(hit);


            if (hit.knockback > 0)
                player.Knockback(hit.source.transform.position, hit.knockback, 0.5f);


            player.ApplyDamage(hit.damage);
        }
    }
}
