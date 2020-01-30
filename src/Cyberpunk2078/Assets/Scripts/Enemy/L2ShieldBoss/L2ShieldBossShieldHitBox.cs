﻿using UnityEngine;


public class L2ShieldBossShieldHitBox : ShieldHitBox
{
    [Header("Normal Knockback")]
    [SerializeField] private float normalKnockbackDuration = 0.5f;
    [SerializeField] private float normalKnockbackFatigue = 0f;
    [Header("Large Knockback")]
    [SerializeField] private float largeKnockbackMultiplier = 5f;
    [SerializeField] private float largeKnockbackDuration = 1.5f;
    [SerializeField] private float largeKnockbackFatigue = 2f;


    protected override void OnHitPlayerHitBox(PlayerCharacter player)
    {
        L2ShieldBoss shieldBoss = (L2ShieldBoss)hit.source;


        shieldBoss.OnAttack.Invoke();
        player.OnHit?.Invoke(hit);


        player.ApplyDamage(hit.damage);


        if (player.State.Name == "Dashing")
        {
            player.Knockback(shieldBoss.transform.position, largeKnockbackMultiplier * hit.knockback, largeKnockbackDuration);


            shieldBoss.ApplyFatigue(largeKnockbackFatigue);
            shieldBoss.FSM.CurrentStateName = "PowerGuard";
        }
        else
        {
            player.Knockback(shieldBoss.transform.position, hit.knockback, normalKnockbackDuration);


            shieldBoss.ApplyFatigue(normalKnockbackFatigue);
        }
    }
}
