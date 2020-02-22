using UnityEngine;


public class LinearMovement : Movement
{
    public float speed;
    public Vector3 orientation;
    public Vector3 initialPosition;
    public float spawnTime = 0;

    public bool isPersistent = true;

    private void OnEnable()
    {
        orientation = orientation.normalized;
        transform.right = orientation;
        transform.position = initialPosition;

        if (spawnTime == 0)
            spawnTime = Time.time;
    }

    private void OnDisable()
    {
        spawnTime = 0;
    }

    private void Update()
    {
        transform.position += Time.deltaTime * TimeManager.Instance.TimeFactor * orientation * speed;
        if(isPersistent == false)
        {
            if(Time.time - spawnTime > 8f)
            {
                Destroy(this);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawLine(transform.position, transform.position + orientation.normalized);
    }
#endif
}

