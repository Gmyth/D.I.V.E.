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
        rb2d.angularVelocity = 0;
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


    public void Throw(int objectID)
    {
        Vector3 handOffset = new Vector3(1, 2, 0);
        handOffset.Scale(transform.localScale);


        Explosive projectile = ObjectRecycler.Singleton.GetObject<Explosive>(objectID);
        projectile.source = this;


        ParabolaMovement projectileMovement = projectile.GetComponent<ParabolaMovement>();
        projectileMovement.targetTime = 1f;
        projectileMovement.g = 30;
        projectileMovement.initialPosition = transform.position + handOffset;
        projectileMovement.targetPosition = currentTarget.transform.position;


        projectile.gameObject.SetActive(true);
    }


    public override float ApplyDamage(float rawDamage)
    {
        if (isInvulnerable)
            return 0;


        if (isEvading)
        {
            isEvading = false;


            if (hitBoxes[2].isActiveAndEnabled || hitBoxes[3].isActiveAndEnabled)
            {
                Debug.LogFormat("[L2ShieldBoss] Blocked an incoming attack.");

                return 0;
            }
            

            return ApplyFatigue(rawDamage * statistics.Sum(AttributeType.Fatigue_p0) * (1 + statistics.Sum(AttributeType.Fatigue_p1)));
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
        if (rawFatigue <= 0 || fsm.CurrentStateName == "Tired" || fsm.CurrentStateName == "DiveBomb")
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


        anger = 0;
    }


    protected override void Update()
    {
        anger = Mathf.Max(0, anger - angerDecrease * TimeManager.Instance.ScaledDeltaTime);


        base.Update();
    }


    private void HandleHit(Hit hit, Collider2D collider)
    {
        if (!isInvulnerable)
        {
            PlayerCharacter player = hit.source.GetComponent<PlayerCharacter>();

            Vector3 position = transform.position;
            Vector3 playerPosition = player.transform.position;
            

            float hitAngle = 0;

            switch (hit.type)
            {
                case Hit.Type.Melee:
                    hitAngle = Vector3.Angle(transform.localScale.x * transform.right, playerPosition - position);
                    break;


                case Hit.Type.Bullet:
                    hitAngle = Vector3.Angle(transform.localScale.x * transform.right, -hit.bullet.GetComponent<LinearMovement>().orientation);
                    break;
            }


            if (hitAngle < 90)
                isEvading = true;
            else if (fsm.CurrentStateName != "Hurt")
            {
                ObjectRecycler.Singleton.GetSingleEffect(15, transform);


                if (fsm.CurrentStateName == "Tired")
                {
                    TimeManager.Instance.startSlowMotion(0.4f, 0.7f, 0.2f);
                    CameraManager.Instance.FlashIn(7.5f, 0.1f, 0.7f, 0.2f);


                    ApplyDamage(1);
                    // statusModifiers.Modify(AttributeType.MaxFatigue_m0, 1);


                    fsm.CurrentStateName = "Alert";
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
                    else if (fsm.CurrentStateName != "ChargedDash" && fsm.CurrentStateName != "CounterAttack")
                        fsm.CurrentStateName = "Hurt";
                }
            }


            Vector3 knockbackDirection = playerPosition.x > position.x ? Vector3.right : Vector3.left;

            Knockback(-knockbackDirection, 30);
            player.Knockback(Vector3.up + knockbackDirection, 20, 0);
        }
    }
}
