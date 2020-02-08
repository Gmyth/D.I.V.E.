using UnityEngine;


public class ParabolaMovement : Movement
{
    public float targetTime;
    public Vector3 initialPosition;
    public Vector3 targetPosition;
    public float g = 10;

    private Vector3 v0;
    private Vector3 vg;
    private float t = 0;


    private void OnEnable()
    {
        transform.position = initialPosition;

        v0 = new Vector3((targetPosition.x - initialPosition.x) / targetTime, (targetPosition.y - initialPosition.y) / targetTime + 0.5f * g * targetTime, (targetPosition.z - initialPosition.z) / targetTime);
        vg = Vector3.zero;
    }
    
    private void Update()
    {
        float dt = TimeManager.Instance.ScaledDeltaTime;

        vg.y = -g * (t += dt);
        transform.Translate((v0 + vg) * dt);
    }
}
