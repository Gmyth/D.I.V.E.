using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Airborne", menuName = "Player State/Airborne")]
public class PSAirborne : PlayerState
{
    [SerializeField] private int indexPSIdle;
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSWallJumping;
    [SerializeField] private int indexPSJumping2;
    [SerializeField] private int indexPSDashing;
    [SerializeField] private int indexPSClimb;

    public override int Update()
    {
        if (Input.GetAxis("Attack1") > 0)
        {
            return indexPSAttackGH;
        }
        
        if (isCloseTo("Ladder") && Input.GetAxis("Vertical") > 0 )
        {
            // up is pressed
            return indexPSClimb;
        }
        
        if (isCloseTo("Ground"))
        {
            return indexPSWallJumping;
        }
        
        if (Input.GetAxis("Jump") > 0)
            return indexPSJumping2;
        
        if (Input.GetAxis("Dashing") != 0)
            return indexPSDashing;
        
        if (isGrounded())
            return Input.GetAxis("Horizontal") != 0?indexPSMoving:indexPSIdle;
        
        return Index;
    }
    
    public override void OnStateEnter()
    {
        anim.Play("MainCharacter_Airborne", -1, 0f);
    }
}
