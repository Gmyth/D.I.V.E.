using UnityEngine;


[CreateAssetMenuAttribute(fileName = "L2DroneState_Firing", menuName = "Enemy State/Drone/Firing")]
public class DroneState_Firing : ESChargedAttack<Drone>
{
    [SerializeField] private float recoil = 10;

    private Rigidbody2D rigidbody;
    private Animator gunAnimator;


    public override void Initialize(Drone enemy)
    {
        base.Initialize(enemy);


        rigidbody = enemy.GetComponent<Rigidbody2D>();
        gunAnimator = enemy.Gun.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        rigidbody.velocity = Vector2.zero;


        AdjustFacingDirection((enemy.currentTarget.transform.position - enemy.transform.position).x > 0 ? Vector3.right : Vector3.left);


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


    protected override int Attack(float currentTime)
    {
        enemy.Fire();


        rigidbody.AddForce(recoil * -enemy.AimingDirection, ForceMode2D.Impulse);


        return stateIndex_alert;
    }
}
