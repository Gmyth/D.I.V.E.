using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2ShieldBossState_Shockwave", menuName = "Enemy State/Shield Boss/Shock Wave")]
public class L2ShieldBossState_Shockwave : ESChargedAttack<L2ShieldBoss>
{
    private float t_attack;


    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_attack = 0;


        enemy.StopTurning();

        AudioManager.Singleton.PlayOnce("Boss_jumpvoice");
    }


    protected override string Attack(float currentTime)
    {
        

        if (t_attack == 0)
        {
            enemy.EmitShockwave();
            animator.Play(animation_attack);

            AudioManager.Singleton.PlayOnce("Boss_attackwave");
        }


        t_attack += TimeManager.Instance.ScaledDeltaTime;

        if (t_attack >= attackBackswing)
            return state_afterAttack;


        return Name;
    }
}
