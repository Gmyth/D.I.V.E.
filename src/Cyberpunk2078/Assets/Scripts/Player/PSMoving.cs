using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Moving", menuName = "Player State/Moving")]
public class PSMoving : PlayerState
{
    [SerializeField] private float speed_factor = 3;
    [SerializeField] private int index_PSIdle;
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


        float x = Input.GetAxis("Horizontal");

        Debug.LogWarning(x);

        if (x == 0)
            return index_PSIdle;

        Move(x);


        if (Input.GetAxis("Jump") > 0)
            return index_PSJumping1;

        return Index;
    }


    public override void OnStateEnter()
    {
        Move(Input.GetAxis("Horizontal"));
    }


    internal void Move(float axis)
    {
        int direction = axis > 0 ? 1 : -1;
        PhysicsInputHelper(axis,speed_factor);
//        Vector2 V = playerCharacter.GetComponent<Rigidbody2D>().velocity;
//
//        V.x = direction * speed_factor;
//        playerCharacter.GetComponent<Rigidbody2D>().velocity = V;
    }
}
