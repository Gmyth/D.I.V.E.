using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_Shockwave", menuName = "Enemy State/Shield Boss/Shock Wave")]
public class L2ShieldBossState_Shockwave : ESChargedAttack<L2ShieldBoss>
{
    [Header("Attack")]
    [SerializeField] private float motionTime = 1.5f;
    [SerializeField] private string animation_attack = "L2ShieldBoss_Shockwave1";

    private float t_attack;


    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_attack = 0;


        enemy.StopTurning();
    }


    protected override string Attack(float currentTime)
    {
        if (t_attack == 0)
        {
            enemy.EmitShockwave();
            animator.Play(animation_attack);
        }


        t_attack += TimeManager.Instance.ScaledDeltaTime;

        if (t_attack >= motionTime)
            return state_afterAttack;


        return Name;
    }
}
