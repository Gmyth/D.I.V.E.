using System;
using UnityEngine;


public abstract class Dummy : MonoBehaviour, IDamageable
{
    public void Start()
    {
        
    }


    public abstract float ApplyDamage(int instanceId, float rawDamage, bool overWrite = false);

    public abstract void Dead();
}
