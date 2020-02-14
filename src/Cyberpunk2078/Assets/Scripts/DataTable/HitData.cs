using System;
using UnityEngine;


[Serializable] public class HitData : DataTableEntry
{
    [SerializeField] private int id;
    [SerializeField] private Hit.Type type;
    [SerializeField] private float damage;
    [SerializeField] private float damageRange;
    [SerializeField] private float damageDuration;
    [SerializeField] private float knockback;
    [SerializeField] private float knockbackDuration;
    

    public override int Index
    {
        get
        {
            return id;
        }
    }

    public int Id
    {
        get
        {
            return id;
        }
    }

    public Hit.Type Type
    {
        get
        {
            return type;
        }
    }

    public float Damage
    {
        get
        {
            return damage;
        }
    }

    public float DamageRange
    {
        get
        {
            return damageRange;
        }
    }

    public float DamageDuration
    {
        get
        {
            return damageDuration;
        }
    }

    public float Knockback
    {
        get
        {
            return knockback;
        }
    }

    public float KnockbackDuration
    {
        get
        {
            return knockbackDuration;
        }
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}
