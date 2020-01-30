using UnityEngine;


public class L2ShieldBoss : Enemy
{
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
        if (fsm.CurrentStateName == "Tired" || fsm.CurrentStateName == "DiveBomb" || rawFatigue <= 0)
            return 0;


        StatisticModificationResult result = statistics.Modify(StatisticType.Fatigue, rawFatigue, 0, statistics[StatisticType.MaxFatigue]);


        Debug.LogFormat("[L2ShieldBoss] Fatigue: {0} / {1}", result.currentValue, statistics[StatisticType.MaxFatigue]);


        if (result.currentValue >= statistics[StatisticType.MaxFatigue])
        {
            if (statusModifiers[AttributeType.MaxFatigue_m0] == 1)
                fsm.CurrentStateName = "DiveBomb";
            else
                fsm.CurrentStateName = "Tired";


            statistics[StatisticType.Fatigue] = 0;
        }


        return result.currentValue - result.previousValue;
    }


    protected override void Start()
    {
        base.Start();


        OnHit.AddListener(HandleHit);


        currentTarget = PlayerCharacter.Singleton;

        isInvulnerable = true;
    }


    private void HandleHit(Hit hit, Collider2D collider)
    {
        if (hitBoxes[2].isActiveAndEnabled || hitBoxes[3].isActiveAndEnabled)
        {
            switch (hit.type)
            {
                case Hit.Type.Melee:
                    if (Vector3.Angle(transform.localScale.x * transform.right, hit.source.transform.position - transform.position) < 90)
                        isEvading = true;
                    break;


                case Hit.Type.Projectile:
                    if (Vector3.Angle(transform.localScale.x * transform.right, -hit.bullet.GetComponent<LinearMovement>().orientation) < 90)
                        isEvading = true;
                    break;
            }
        }
        else
        {
            ObjectRecycler.Singleton.GetSingleEffect(15, transform);

            TimeManager.Instance.startSlowMotion(0.5f, 0.2f);
        }
    }
}
