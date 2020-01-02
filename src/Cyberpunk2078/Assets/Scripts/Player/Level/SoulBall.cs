using UnityEngine;


public enum SoulBallState
{
    Move,
    Chase,
    Acquired
}


public class SoulBall : Recyclable
{
    [SerializeField] private float maxSpeedIn = 30f;
    [SerializeField] private float accelerationIn = 20f;
    [SerializeField] private float minSpeedOut = 5f;
    [SerializeField] private float accelerationOut = 10f;

    private Transform target;
    private Rigidbody2D rb2d;

    private SoulBallState currentState;
    private Vector2 outDir;
    private float t;
    private float v;
    private float timeIntervals = 0.6f;
    private bool triggered;


    private void OnEnable()
    {
        target = PlayerCharacter.Singleton.transform;
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.velocity = Vector2.zero;
        
        triggered = false;
        currentState = SoulBallState.Move;
        t = Time.time + timeIntervals + Random.Range(-0.2f, 0.2f);
        v = Random.Range(6f, 10f);

        outDir = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
    }

    private void Update()
    {
        switch (currentState)
        {
            case SoulBallState.Move:
                if (Time.time >= t || v <= 1)
                {
                    currentState = SoulBallState.Chase;
                    v = -v;
                    //rb2d.velocity = Vector2.zero;
                }
                else
                {
                    transform.Translate(outDir.normalized * v * Time.deltaTime);
                    v = Mathf.Max(minSpeedOut, v - accelerationOut * Time.deltaTime);
                }

                break;
                

            case SoulBallState.Chase:
                Vector3 direction = target.position - transform.position;

                if (direction.sqrMagnitude < 0.1f)
                {
                    rb2d.velocity = Vector2.zero;

                    //rb2d.drag = 0;
                    //bufferState = SoulBallState.Acquired;

                    PlayerCharacter.Singleton.AddUltimateEnergy(4f);
                    triggered = true;
                    Die();
                }
                else if(!triggered)
                {
                    float distance = v * Time.deltaTime;

                    if (direction.sqrMagnitude <= distance * distance)
                        transform.position = target.position;
                    else
                        transform.Translate(direction.normalized * distance);

                    v = Mathf.Min(maxSpeedIn, v + accelerationIn * Time.deltaTime);
                }
                break;
        }
    }
}
