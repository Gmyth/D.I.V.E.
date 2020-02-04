using UnityEngine;


public class L2ShieldBoss : Enemy
{
    [SerializeField] private float angerDecrease = 5f;
    [SerializeField] private float counterThreshhold = 20f;
    public GameObject spark;

    private float anger = 0;


    public void Knockback(Vector3 direction, float force)
    {
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(force * direction.normalized, ForceMode2D.Impulse);
    }

    public void EmitShockwave()
    {
        EmitShockwave(17, transform.localScale, true);
    }

    public void EmitShockwave(bool cameraShake)
    {
        EmitShockwave(17, transform.localScale, cameraShake);
    }

    public void EmitShockwave(int id, Vector3 direction, bool cameraShake = true)
    {
        LinearMovement shockwaveMovement = ObjectRecycler.Singleton.GetObject<LinearMovement>(id);
        shockwaveMovement.initialPosition = transform.position + new Vector3(1, 0, 0);
        shockwaveMovement.orientation = direction.x > 0 ? Vector3.right : Vector3.left;


        HitBox shockwaveHitbox = shockwaveMovement.GetComponent<HitBox>();

        if (shockwaveHitbox)
            shockwaveHitbox.hit.source = this;


        shockwaveMovement.gameObject.SetActive(true);


        if (cameraShake)
            CameraManager.Instance.Shaking(0.2f, 0.2f);
    }


    public override float ApplyDamage(float rawDamage)
    {
        if (isEvading)
        {
            Debug.LogFormat("[L2ShieldBoss] Blocked an incoming attack.");
            isEvading = false;
            return 0;
        }


        if (isInvulnerable)
        {
            ApplyFatigue(rawDamage * statistics.Sum(AttributeType.Fatigue_p0) * (1 + statistics.Sum(AttributeType.Fatigue_p1)));
            return 0;
        }


        StatisticModificationResult result = statistics.Modify(StatisticType.Hp, -rawDamage, 0, statistics[StatisticType.MaxHp]);

        if (result.currentValue <= 0)
            Dead();


        return result.previousValue - result.currentValue;
    }

    public override void Dead()
    {
        throw new System.NotImplementedException();
    }

    public float ApplyFatigue(float rawFatigue)
    {
        if (rawFatigue <= 0 || fsm.CurrentStateName == "Tired" || fsm.CurrentStateName == "DiveBomb" || fsm.CurrentStateName == "CounterAttack")
            return 0;


        StatisticModificationResult result = statistics.Modify(StatisticType.Fatigue, rawFatigue, 0, statistics[StatisticType.MaxFatigue]);


        Debug.LogFormat("[L2ShieldBoss] Fatigue: {0} / {1}", result.currentValue, statistics[StatisticType.MaxFatigue]);


        if (result.currentValue >= statistics[StatisticType.MaxFatigue])
        {
            if (statusModifiers[AttributeType.MaxFatigue_m0] == 1)
                fsm.CurrentStateName = "DiveBombReposition";
            else
                fsm.CurrentStateName = "Tired";


            statistics[StatisticType.Fatigue] = 0;
        }
        else if (fsm.CurrentStateName != "Knockback" && fsm.CurrentStateName != "DiveBomb")
            anger += rawFatigue;


        return result.currentValue - result.previousValue;
    }


    protected override void Start()
    {
        currentTarget = PlayerCharacter.Singleton;


        base.Start();


        rb2d = GetComponent<Rigidbody2D>();


        OnHit.AddListener(HandleHit);


        isInvulnerable = true;
        anger = 0;
    }


    protected override void Update()
    {
        anger = Mathf.Max(0, anger - angerDecrease * TimeManager.Instance.ScaledDeltaTime);


        base.Update();
    }


    private void HandleHit(Hit hit, Collider2D collider)
    {
        float hitAngle = 0;
            
        switch (hit.type)
        {
            case Hit.Type.Melee:
                hitAngle = Vector3.Angle(transform.localScale.x * transform.right, hit.source.transform.position - transform.position);
                break;


            case Hit.Type.Projectile:
                hitAngle = Vector3.Angle(transform.localScale.x * transform.right, -hit.bullet.GetComponent<LinearMovement>().orientation);
                break;
        }
        
        
        if ((hitBoxes[2].isActiveAndEnabled || hitBoxes[3].isActiveAndEnabled) && hitAngle < 90)
            isEvading = true;
        else if (fsm.CurrentStateName != "Hurt")
        {
            ObjectRecycler.Singleton.GetSingleEffect(15, transform);


            if (fsm.CurrentStateName == "Tired")
            {
                TimeManager.Instance.startSlowMotion(0.4f, 0.7f, 0.2f);
                CameraManager.Instance.FlashIn(7.5f, 0.1f, 0.7f, 0.2f);


                isInvulnerable = false;

                ApplyDamage(1);
                statusModifiers.Modify(AttributeType.MaxFatigue_m0, 1);

                isInvulnerable = true;


                fsm.CurrentStateName = "Knockback";
            }
            else if (fsm.CurrentStateName != "Knockback" && fsm.CurrentStateName != "DiveBombReposition" && fsm.CurrentStateName != "DiveBomb")
            {
                TimeManager.Instance.startSlowMotion(0.4f, 0.7f, 0.2f);
                CameraManager.Instance.FlashIn(7.5f, 0.1f, 0.7f, 0.2f);

                
                if (anger >= 20)
                {
                    fsm.CurrentStateName = "CounterAttack";
                    anger = 0;
                }
                else if (fsm.CurrentStateName != "ChargedDash" && fsm.CurrentStateName != "CounterAttack" && hitAngle > 90)
                    fsm.CurrentStateName = "Hurt";
            }
        }
    }
}
