using System;
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
    
    [Header("Bounce Config")]
    public bool Bounce = true;
    private int bounceCounter = 0;
    public float bounceRatio;
    public int maxBounceTimes;
    
    private float lastHitEstimation;
    private bool hunchTriggered;

    private int numHitsRemaining;


    protected override void OnEnable()
    {
        base.OnEnable();
        hunchTriggered = false;
        numHitsRemaining = numHits;
        bounceCounter = 0;
        GetComponent<TrailRenderer>().Clear();
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (Bounce)
        {
            if (bounceCounter > 3)
            {
                numHitsRemaining = 0;
                return;
            }
                
            GetComponent<LinearMovement>().initialPosition = transform.position;
            GetComponent<LinearMovement>().speed *= bounceRatio;
            GetComponent<LinearMovement>().orientation = Vector3.Reflect(GetComponent<LinearMovement>().orientation,other.contacts[0].normal);
            GetComponent<LinearMovement>().spawnTime = Time.time;
            bounceCounter++;
            transform.right = GetComponent<LinearMovement>().orientation;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
            Hit.transform.right = transform.right;
            Hit.transform.position = transform.position;
            Hit.transform.localScale = Vector3.one;

            Hit.gameObject.SetActive(true);

            if (!Bounce || bounceCounter > maxBounceTimes) numHitsRemaining = 0;
        }
        else if (isFriendly)
        {
            if (other.tag == "Enemy")
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
                hitFx.transform.position = other.transform.position + (transform.position - other.transform.position) * 0.5f;
                hitFx.transform.localScale = Vector3.one;

                hitFx.gameObject.SetActive(true);
            }
        }
        else if (other.tag == "Player")
        {
            PlayerCharacter player = other.GetComponent<PlayerCharacter>();

            if (player.State.Index != 6)
            {
                AudioManager.Singleton.PlayOnce("Hit_by_laser");
                //Not in dash, deal damage
                player.ApplyDamage(rawDamage);

                if (hit.knockback > 0)
                    player.KnockbackHorizontal(transform.position, hit.knockback, 0.3f);


                --numHitsRemaining;


                //TimeManager.Instance.endSlowMotion();
                CameraManager.Instance.Idle();

                CameraManager.Instance.Shaking(0.20f, 0.05f);
                SingleEffect Hit = ObjectRecycler.Singleton.GetObject<SingleEffect>(4);
                Hit.transform.right = transform.right;
                Hit.transform.position = other.transform.position + (transform.position - other.transform.position) * 0.5f;
                Hit.transform.localScale = Vector3.one;
                Hit.gameObject.SetActive(true);
            }
        }
        else if (other.tag == "PlayerHitBox")
        {
            if (other.name != "DashAtkBox")
            {
                isFriendly = true;
                GetComponent<LinearMovement>().initialPosition = transform.position;
                //GetComponent<LinearMovement>().speed *= 1.5f;
                GetComponent<LinearMovement>().orientation = GetComponent<LinearMovement>().orientation * -1;
                GetComponent<LinearMovement>().spawnTime = Time.time;
                
                GetComponent<TrailRenderer>().Clear();
                
                var spark = ObjectRecycler.Singleton.GetObject<SingleEffect>(23);
                spark.transform.position = transform.position;
                spark.transform.right = transform.right;
                spark.transform.localScale = Vector3.one;
                spark.gameObject.SetActive(true);
                
                transform.right = GetComponent<LinearMovement>().orientation;

                //TimeManager.Instance.endSlowMotion();
                CameraManager.Instance.Idle();
                TimeManager.Instance.startSlowMotion(0.8f);
                CameraManager.Instance.Shaking(0.5f, 0.05f, true);
                CameraManager.Instance.FocusTo(transform.position, 0.02f);

                SingleEffect Hit1 = ObjectRecycler.Singleton.GetObject<SingleEffect>(12);
                Hit1.transform.right = transform.right;
                Hit1.transform.position = other.transform.position + (transform.position - other.transform.position) * 0.3f;

                Hit1.gameObject.SetActive(true);

                AudioManager.Singleton.PlayOnce("DeflectBullet");
            }
        }


        if (numHitsRemaining <= 0)
            Die();
    }
}
