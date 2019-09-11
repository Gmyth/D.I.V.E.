using UnityEngine;


public abstract class Dummy : MonoBehaviour, IDamageable
{
    public abstract float ApplyDamage(float rawDamage);
}
