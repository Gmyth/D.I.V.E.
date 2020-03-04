using UnityEngine;


public class L2ShieldBoss : Enemy
{
    [Header("Field")]
    [SerializeField] private Vector3 leftThrowPoint;
    [SerializeField] private Vector3 rightThrowPoint;
    [SerializeField] private EnemySpawner leftDroneSpawner;
    [SerializeField] private EnemySpawner rightDroneSpawner;

    [Header("On Hit")]
    [SerializeField] private float knockbackOnHit = 30;
    [SerializeField] private float knockbackOnHitFromBack = 50;
    [SerializeField] private float playerKnockbackOnHit = 20;
    [SerializeField] private float playerKnockbackOnHitFromBack = 15;

    [Header("Rage")]
    [SerializeField][Range(0, 1)] private float ragePercentage = 0.5f;
    [SerializeField] private float angerDecrease = 5f;
    [SerializeField] private float counterThreshhold = 20f;

    [Header("References")]
    [SerializeField] private Transform handAnchor;
    [SerializeField] private Transform laser;
    [SerializeField] private SimpleBreakable glass;
    public GameObject spark;

    private float anger = 0;
    private bool inRage = false;


    public Vector3 LeftThrowPoint
    {
        get
        {
            return leftThrowPoint;
        }
    }

    public Vector3 RightThrowPoint
    {
        get
        {
            return rightThrowPoint;
        }
    }

    public EnemySpawner LeftDroneSpawner
    {
        get
        {
            return leftDroneSpawner;
        }
    }

    public EnemySpawner RightDroneSpawner
    {
        get
        {
            return rightDroneSpawner;
        }
    }

    public Transform HandAnchor
    {
        get
        {
            return handAnchor;
        }
    }

    public bool IsGuarding
    {
        get
        {
            return hitBoxes[2].isActiveAndEnabled || hitBoxes[3].isActiveAndEnabled;
        }
    }


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

    public void EmitShockwave(int objectID, Vector3 direction, bool cameraShake = true)
    {
        LinearMovement shockwaveMovement = ObjectRecycler.Singleton.GetObject<LinearMovement>(objectID);
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


    public void InitiateDroneSpawning()
    {
        rightDroneSpawner.gameObject.SetActive(true);
        leftDroneSpawner.gameObject.SetActive(true);

        glass?.DestroyBreakable(glass.transform.position + new Vector3(0, 2, 0));
        glass = null;
    }


    public override float ApplyDamage(float rawDamage)
    {
        if (IsInvulnerable)
            return 0;


        if (isEvading)
        {
            isEvading = false;


            if (IsGuarding)
            {
                Debug.LogFormat("[L2ShieldBoss] Blocked an incoming attack.");

                return 0;
            }
            

            return ApplyFatigue(rawDamage * statistics.Sum(AttributeType.Fatigue_p0) * (1 + statistics.Sum(AttributeType.Fatigue_p1)));
        }


        statistics[StatisticType.Fatigue] = 0;


        StatisticModificationResult result = statistics.Modify(StatisticType.Hp, -rawDamage, 0, statistics[StatisticType.MaxHp]);


        Debug.LogFormat("[L2ShieldBoss] HP: {0} / {1}", result.currentValue, statistics[StatisticType.MaxHp]);


        if (result.currentValue <= 0)
            Dead();
        else if (!inRage && result.currentValue <= ragePercentage * statistics[StatisticType.MaxHp])
        {
            inRage = true;
            fsm.CurrentStateName = "Rage";
        }

        return result.previousValue - result.currentValue;
    }

    public override void Dead()
    {
        Destroy(gameObject);
        GUIManager.Singleton.Open("EndScreen");
    }

    public float ApplyFatigue(float rawFatigue)
    {
        if (rawFatigue <= 0 || fsm.CurrentStateName == "Tired" || fsm.CurrentStateName == "DiveBomb")
            return 0;


        StatisticModificationResult result = statistics.Modify(StatisticType.Fatigue, rawFatigue, 0, statistics[StatisticType.MaxFatigue]);


        Debug.LogFormat("[L2ShieldBoss] Fatigue: {0} / {1}", result.currentValue, statistics[StatisticType.MaxFatigue]);


        if (result.currentValue >= statistics[StatisticType.MaxFatigue])
        {
            fsm.CurrentStateName = "Tired";

            statistics[StatisticType.Fatigue] = 0;
        }
        else if (fsm.CurrentStateName != "Knockback" && fsm.CurrentStateName != "DiveBomb")
            anger += rawFatigue;


        return result.currentValue - result.previousValue;
    }


    public override void Reset()
    {
        base.Reset();


        anger = 0;
        inRage = false;


        rightDroneSpawner.Clear();
        rightDroneSpawner.gameObject.SetActive(false);
        leftDroneSpawner.Clear();
        leftDroneSpawner.gameObject.SetActive(false);
    }


    public override void TurnImmediately(Vector3 direction)
    {
        base.TurnImmediately(direction);


        if (enableTurn)
            laser.rotation = direction.x > 0 ? Quaternion.identity : Quaternion.Euler(0, 0, 180);
    }


    protected override void Awake()
    {
        base.Awake();


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
        if (!IsInvulnerable)
        {
            PlayerCharacter player = hit.source.GetComponent<PlayerCharacter>();

            Vector3 position = transform.position;
            Vector3 playerPosition = player.transform.position;
            Vector3 knockbackDirection = playerPosition.x > position.x ? Vector3.right : Vector3.left;


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


            if (fsm.CurrentStateName == "Tired")
            {
                ObjectRecycler.Singleton.GetSingleEffect(15, transform, new Vector3(0, 1, 0));


                TimeManager.Instance.startSlowMotion(0.4f, 0.7f, 0.2f);
                CameraManager.Instance.FlashIn(7.5f, 0.1f, 0.7f, 0.2f);


                Knockback(-knockbackDirection, knockbackOnHitFromBack);
                player.Knockback(Vector3.up + knockbackDirection, playerKnockbackOnHitFromBack, 0);


                fsm.CurrentStateName = "Hurt";
            }
            else if (hitAngle < 90)
            {
                isEvading = true;


                if (!IsGuarding)
                {
                    Knockback(-knockbackDirection, knockbackOnHit);
                    player.Knockback(Vector3.up + knockbackDirection, playerKnockbackOnHit, 0);


                    fsm.CurrentStateName = "InstantGuard";
                }
            }
            else if (fsm.CurrentStateName != "Knockback" && fsm.CurrentStateName != "DiveBombReposition" && fsm.CurrentStateName != "DiveBomb")
            {
                ObjectRecycler.Singleton.GetSingleEffect(15, transform, new Vector3(0, 1, 0));


                TimeManager.Instance.startSlowMotion(0.4f, 0.7f, 0.2f);
                CameraManager.Instance.FlashIn(7.5f, 0.1f, 0.7f, 0.2f);


                Knockback(-knockbackDirection, knockbackOnHitFromBack);
                player.Knockback(Vector3.up + knockbackDirection, playerKnockbackOnHitFromBack, 0);


                if (anger >= 20)
                {
                    fsm.CurrentStateName = "CounterAttack";
                    anger = 0;
                }
                else if (fsm.CurrentStateName != "ChargedDash" && fsm.CurrentStateName != "CounterAttack")
                    fsm.CurrentStateName = "Hurt";
            }
        }
    }
}
