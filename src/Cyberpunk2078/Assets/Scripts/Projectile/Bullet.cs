using UnityEngine;


[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Bullet : Recyclable
{
    public bool isFriendly = false;
    public int numHits = 1;
    public int rawDamage = 1;
    public Hit hit;

    [SerializeField] private float hitEstimationTimeInterval = 0.02f;

    [SerializeField] private bool disableHunch;
    private float lastHitEstimation;
    private bool hunchTriggered;

    private int numHitsRemaining;


    protected override void OnEnable()
    {
        base.OnEnable();
        hunchTriggered = false;
        numHitsRemaining = numHits;
    }


    private void Start()
    {
        hit.bullet = this;
    }

    private void Update()
    {
        GetComponentInChildren<Animator>().speed = TimeManager.Instance.TimeFactor;
        // GetComponent<LinearMovement>().speed *= TimeManager.Instance.TimeFactor;

        if (!disableHunch && lastHitEstimation + hitEstimationTimeInterval < Time.unscaledTime && !hunchTriggered && !isFriendly )
        {
            // time to check
            var direction = GetComponent<LinearMovement>().orientation;
            RaycastHit2D hit = Physics2D.Raycast(transform.position,direction, 4f);
            if (hit.collider != null && hit.transform.CompareTag("Player"))
            {
                //hit! Hunch Trigger
                PlayerCharacter playerCharacter = hit.collider.GetComponent<PlayerCharacter>();
                //if (playerCharacter.InKillStreak)
                //{
                //    hunchTriggered = true;
                //    playerCharacter.AddKillCount(-2);
                //    TimeManager.Instance.startSlowMotion(1f);
                //}
            }

            lastHitEstimation = Time.unscaledTime;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (numHitsRemaining <= 0)
            Die();
        else if (other.tag == "Ground")
        {
            SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
            Hit.transform.right = transform.right;
            Hit.transform.position = transform.position;
            Hit.transform.localScale = Vector3.one;

            Hit.gameObject.SetActive(true);


            Die();
        }
        else if (isFriendly)
        {
            if (other.tag == "Dummy")
            {
                Dummy enemy = other.GetComponent<Dummy>();


                enemy.OnHit?.Invoke(hit, other);


                enemy.ApplyDamage(rawDamage);
                --numHitsRemaining;


                CameraManager.Instance.Shaking(0.20f, 0.05f);
                //TimeManager.Instance.endSlowMotion();
                CameraManager.Instance.Idle();

                SingleEffect hitFx = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                hitFx.transform.position = other.transform.position - (other.transform.position - transform.position) * 0.2f;
                hitFx.transform.right = transform.right;
                hitFx.transform.position = other.transform.position + (transform.position  - other.transform.position) * 0.5f;
                hitFx.transform.localScale = Vector3.one;

                hitFx.gameObject.SetActive(true);


                Die();
            }
            else if (other.tag == "Ground")
            {
                SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.transform.right = transform.right;
                Hit.transform.position = transform.position;
                Hit.transform.localScale = Vector3.one;

                Hit.gameObject.SetActive(true);


                Die();
            }
        }
        else if (other.tag == "Player")
        {
            if (other.GetComponent<PlayerCharacter>().State.Index != 6)
            {
                //Not in dash, deal damage
                other.GetComponent<PlayerCharacter>().ApplyDamage(rawDamage);
                other.GetComponent<PlayerCharacter>().KnockbackHorizontal(transform.position, 300f, 0.3f);
                --numHitsRemaining;

                //TimeManager.Instance.endSlowMotion();
                CameraManager.Instance.Idle();

                CameraManager.Instance.Shaking(0.20f, 0.05f);
                SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.transform.right = transform.right;
                Hit.transform.position = other.transform.position + (transform.position - other.transform.position) * 0.5f;
                Hit.transform.localScale = Vector3.one;

                Hit.gameObject.SetActive(true);

                Die();
            }
        }
        else if (other.tag == "PlayerHitBox")
        {
            if (other.name != "DashAtkBox")
            {
                isFriendly = true;
                GetComponent<LinearMovement>().initialPosition = transform.position;
                GetComponent<LinearMovement>().speed *= 1.5f;
               // GetComponent<LinearMovement>().orientation = (Quaternion.Euler(0, 0,  Random.Range(-15, 15)) * (GetComponent<LinearMovement>().orientation * -1)).normalized;
                GetComponent<LinearMovement>().orientation = other.transform.right;
                GetComponent<LinearMovement>().spawnTime = Time.time;

                transform.right = GetComponent<LinearMovement>().orientation;

                //TimeManager.Instance.endSlowMotion();
                CameraManager.Instance.Idle();

                CameraManager.Instance.Shaking(0.5f,0.05f,true);
                CameraManager.Instance.FocusTo(transform.position,0.02f);

                SingleEffect Hit1 = ObjectRecycler.Singleton.GetObject<SingleEffect>(12);
                Hit1.transform.right = transform.right;
                Hit1.transform.position = other.transform.position + (transform.position  - other.transform.position) * 0.3f;

                Hit1.gameObject.SetActive(true);

                AudioManager.Instance.PlayOnce("DeflectBullet");
            }

        }

        if (numHitsRemaining <= 0)
            Die();
    }
}
