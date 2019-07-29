using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Idle", menuName = "Player State/Idle")]
public class PSIdle : PlayerState
{
    [SerializeField] private int index_PSMoving;
    [SerializeField] private int index_PSAttackGH1;
    [SerializeField] private int index_PSAttackGH2;
    [SerializeField] private int index_PSAttackGU;
    [SerializeField] private int index_PSAttackGD;
    [SerializeField] private int index_PSJumping1;


    public override int Update()
    {
        if (Input.GetAxis("Attack1") > 0)
        {
            float y = Input.GetAxis("Vertical");

            if (y > 0)
                return index_PSAttackGU;
            else if (y < 0)
                return index_PSAttackGD;

            return index_PSAttackGH1;
        }

        if (Input.GetAxis("Attack2") > 0)
        {
            float y = Input.GetAxis("Vertical");

            if (y > 0)
                return index_PSAttackGU;
            else if (y < 0)
                return index_PSAttackGD;

            return index_PSAttackGH2;
        }

        if (Input.GetAxis("Horizontal") != 0)
            return index_PSMoving;

        if (Input.GetAxis("Jump") > 0)
            return index_PSJumping1;

        return Index;
    }

    public override void OnStateEnter()
    {
        Vector2 V = playerCharacter.GetComponent<Rigidbody2D>().velocity;

        V.x = 0;
        playerCharacter.GetComponent<Rigidbody2D>().velocity = V;
    }
}
