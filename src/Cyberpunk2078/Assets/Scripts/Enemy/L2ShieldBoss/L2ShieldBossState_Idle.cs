using UnityEngine;


public class L2ShieldBossState_Idle : ESIdle<L2ShieldBoss>
{
    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        enemy.LeftDroneSpawner.Clear();
        enemy.LeftDroneSpawner.gameObject.SetActive(false);

        enemy.RightDroneSpawner.Clear();
        enemy.RightDroneSpawner.gameObject.SetActive(false);
    }
}
