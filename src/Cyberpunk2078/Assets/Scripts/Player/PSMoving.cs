using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Moving", menuName = "Player State/Moving")]
public class PSMoving : PlayerState
{
    [SerializeField] private float speedFactor = 3;
    [SerializeField] private float accelerationFactor = 20;
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSJumping1;
    [SerializeField] private int indexPSDashing;
    [SerializeField] private int indexPSAirborne;
    [SerializeField] private int indexPSClimb;
    public override int Update()
    {
        
        if (Input.GetAxis("Attack1") > 0)
        {
            return indexPSAttackGH;
        }
        
        if (Input.GetAxis("Dashing") != 0)
            return indexPSDashing;

        if (Input.GetAxis("Vertical") > 0  &&  isCloseTo("Ladder"))
        {
            // up is pressed
            return indexPSClimb;
        }

        float x = Input.GetAxis("Horizontal");

        
        flip = x <= 0;
        if (x == 0)
            
            return indexPSIdle;
        
        playerCharacter.GetComponent<SpriteRenderer>().flipX = flip;
        Move(x);


        if (Input.GetAxis("Jump") > 0)
            return indexPSJumping1;

        return Index;
    }


    public override void OnStateEnter()
    {
        Move(Input.GetAxis("Horizontal"));
        anim.Play("MainCharacter_Run", -1, 0f);
    }


    internal void Move(float axis)
    {
        int direction = axis > 0 ? 1 : -1;
        PhysicsInputHelper(axis,speedFactor,accelerationFactor);
//        Vector2 V = playerCharacter.GetComponent<Rigidbody2D>().velocity;
//
//        V.x = direction * speed_factor;
//        playerCharacter.GetComponent<Rigidbody2D>().velocity = V;
    }
}
