using UnityEngine;


[CreateAssetMenuAttribute(fileName = "DroneState_Searching", menuName = "Enemy State/Drone/Searching")]
public class DroneState_Searching : ESGuard<Drone>
{
    private Animator animator;


    public override void Initialize(Drone enemy)
    {
        base.Initialize(enemy);


        rigidbody = enemy.GetComponent<Rigidbody2D>();
        animator = enemy.GetComponent<Animator>();
    }

    public override void OnStateEnter(State previousState)
    {
        base.OnStateEnter(previousState);


        rigidbody.velocity = Vector2.zero;
    }

    public override void OnStateQuit(State nextState)
    {
        base.OnStateQuit(nextState);


        animator.Play("L2Drone_Idle");
    }
}
