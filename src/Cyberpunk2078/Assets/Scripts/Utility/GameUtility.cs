using UnityEngine;


public struct GameUtility
{
    public static float ApplyDamage(Dummy target, Hit hit, Collider2D collider)
    {
        hit.source?.OnAttack.Invoke(hit, collider);
        target.OnHit?.Invoke(hit, collider);

        TimeManager.Instance.endSlowMotion();
        CameraManager.Instance.Idle();

        return target.ApplyDamage(hit.damage);
    }
}
