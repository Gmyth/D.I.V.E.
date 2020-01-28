public struct GameUtility
{
    public static void ApplyDamage(Hit hit, Dummy target)
    {
        hit.source?.OnAttack.Invoke();
        target.OnHit?.Invoke(hit);

        TimeManager.Instance.endSlowMotion();
        CameraManager.Instance.Idle();

        target.ApplyDamage(hit.damage);
    }
}
