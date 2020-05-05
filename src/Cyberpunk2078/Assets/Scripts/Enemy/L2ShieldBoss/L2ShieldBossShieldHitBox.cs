using UnityEngine;


public class L2ShieldBossShieldHitBox : ShieldHitBox
{
    [Header("Normal Knockback")]
    [SerializeField] private float normalKnockbackFatigue = 0f;

    [Header("Large Knockback")]
    [SerializeField] private float largeKnockbackMultiplier = 5f;
    [SerializeField] private float largeKnockbackDurationMultiplider = 3f;
    [SerializeField] private float largeKnockbackFatigue = 2f;


    protected override void OnHitPlayerHitBox(Collider2D other)
    {
        PlayerCharacter player = (PlayerCharacter)other.GetComponent<HitBox>().hit.source;
        L2ShieldBoss shieldBoss = (L2ShieldBoss)hit.source;

        if (Vector3.Angle(shieldBoss.transform.localScale.x * shieldBoss.transform.right, player.transform.position - shieldBoss.transform.position) < 90)
        {
            AudioManager.Singleton.PlayOnce("Boss_block");

            shieldBoss.OnAttack.Invoke(hit, other);
            player.OnHit?.Invoke(hit, other);


            player.ApplyDamage(hit.damage);


            Vector3 d = player.transform.position - shieldBoss.transform.position;
            Vector3 direction = d.x > 0 ? new Vector3(1, 1, 0) : new Vector3(-1, 1, 0);


            if (player.State.Name == "Dashing")
            {
                player.Knockback(direction, largeKnockbackMultiplier * hit.knockback, largeKnockbackDurationMultiplider * hit.knockbackDuration);


                shieldBoss.ApplyFatigue(largeKnockbackFatigue);
                shieldBoss.FSM.CurrentStateName = "PowerGuard";
            }
            else
            {
                player.Knockback(direction, hit.knockback, hit.knockbackDuration);


                shieldBoss.ApplyFatigue(normalKnockbackFatigue);
            }


            CreateEffect(transform);
        }
    }
}
