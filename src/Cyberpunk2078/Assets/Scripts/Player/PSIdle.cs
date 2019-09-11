using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Idle", menuName = "Player State/Idle")]
public class PSIdle : PlayerState
{
    [SerializeField] private int indexPSMoving;
    [SerializeField] private int indexPSAttackGH;
    [SerializeField] private int indexPSJumping1;
    [SerializeField] private int indexPSDashing;
    [SerializeField] private int indexPSAirborne;
    public override int Update()
    {
        if (Input.GetAxis("Attack1") > 0)
        {
            return indexPSAttackGH;
        }

//        if (Input.GetAxis("Attack2") > 0)
//        {
//            float y = Input.GetAxis("Vertical");
//
//            if (y > 0)
//                return index_PSAttackGU;
//            else if (y < 0)
//                return index_PSAttackGD;
//
//            return index_PSAttackGH2;
//        }

        if (Input.GetAxis("Horizontal") != 0)
            return indexPSMoving;

        if (Input.GetAxis("Jump") > 0)
            return indexPSJumping1;
        
        if (Input.GetAxis("Dashing") != 0)
            return indexPSDashing;


        return Index;
    }

    public override void OnStateEnter()
    {
       
        anim.Play("MainCharacter_Idle", -1, 0f);
        Vector2 V = playerCharacter.GetComponent<Rigidbody2D>().velocity;
        V.x = 0;
        playerCharacter.GetComponent<Rigidbody2D>().velocity = V;
    }
}
