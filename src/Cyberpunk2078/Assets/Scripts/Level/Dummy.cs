using UnityEngine;


public abstract class Dummy : MonoBehaviour, IDamageable
{
    [SerializeField] private float slowMotionFactor;
    
    public abstract float ApplyDamage(float rawDamage);
}
