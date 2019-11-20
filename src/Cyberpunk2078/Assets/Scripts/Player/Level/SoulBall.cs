using UnityEngine;


public enum SoulBallState
{
    Move,
    Chase,
    Acquired
}


public class SoulBall : Recyclable
{
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float acceleration = 20f;

    private Transform target;
    private Rigidbody2D rb2d;

    private SoulBallState currentState;
    private float t;
    private float v;
    private float timeIntervals = 0.6f;


    private void OnEnable()
    {
        target = PlayerCharacter.Singleton.transform;
        rb2d = GetComponent<Rigidbody2D>();


        currentState = SoulBallState.Move;
        t = Time.time + timeIntervals + Random.Range(-0.2f, 0.2f);
        v = 5f;

        
        rb2d.AddForce(Random.Range(15, 40) * new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)));
    }

    private void Update()
    {
        switch (currentState)
        {
            case SoulBallState.Move:
                if (Time.time >= t)
                {
                    currentState = SoulBallState.Chase;
                    rb2d.velocity = Vector2.zero;
                }


                break;
                

            case SoulBallState.Chase:
                Vector3 direction = target.position - transform.position;

                if (direction.sqrMagnitude < 0.3f)
                {
                    rb2d.velocity = Vector2.zero;

                    //rb2d.drag = 0;
                    //bufferState = SoulBallState.Acquired;

                    PlayerCharacter.Singleton.AddFever(4f);

                    Die();
                }


                float distance = v * Time.deltaTime;

                if (direction.sqrMagnitude <= distance * distance)
                    transform.position = target.position;
                else
                    transform.Translate(direction.normalized * distance);

                v = Mathf.Min(maxSpeed, v + acceleration * Time.deltaTime);
                

                break;
        }
    }
}
