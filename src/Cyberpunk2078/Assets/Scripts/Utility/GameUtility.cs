using UnityEngine;


public struct GameUtility
{
    public static void AdjustFacing(Dummy dummy, Vector2 direction)
    {
        Vector3 scale = dummy.transform.localScale;
        scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);

        dummy.transform.localScale = scale;
    }

    public static void AdjustFacing(Dummy dummy, Vector3 direction)
    {
        Vector3 scale = dummy.transform.localScale;
        scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);

        dummy.transform.localScale = scale;
    }

    public static float ApplyDamage(Dummy target, Hit hit, Collider2D collider)
    {
        hit.source?.OnAttack.Invoke(hit, collider);
        target.OnHit?.Invoke(hit, collider);

        CameraManager.Instance.Idle();

        return target.ApplyDamage(hit.damage);
    }
}
