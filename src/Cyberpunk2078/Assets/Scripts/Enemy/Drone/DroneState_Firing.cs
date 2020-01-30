using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2DroneState_Firing", menuName = "Enemy State/Drone/Firing")]
public class DroneState_Firing : ESChargedAttack<Drone>
{
    [Header("Firing")]
    [SerializeField] private float recoil = 10;
    [SerializeField] protected float recoilDuration = 0.2f;

    private Rigidbody2D rigidbody;
    private Animator gunAnimator;

    private float t_motion;


    public override void Initialize(Drone enemy)
    {
        base.Initialize(enemy);


        rigidbody = enemy.GetComponent<Rigidbody2D>();
        gunAnimator = enemy.Gun.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        t_motion = 0;


        rigidbody.velocity = Vector2.zero;


        AdjustFacingDirection();


        enemy.Aim(enemy.currentTarget.transform.position);


        enemy.isGunCharging = true;

        gunAnimator.Play("L2Drone_Gun_Charging");
    }

    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);


        enemy.isGunCharging = false;

        gunAnimator.Play("L2Drone_Gun_Idle");
    }


    protected override string Attack(float currentTime)
    {
        float t = Time.time;


        if (t_motion == 0)
        {
            enemy.Fire();


            rigidbody.AddForce(recoil * -enemy.AimingDirection, ForceMode2D.Impulse);


            t_motion = t + recoilDuration;
        }

        
        return t >= t_motion ? "Alert" : Name;
    }
}
