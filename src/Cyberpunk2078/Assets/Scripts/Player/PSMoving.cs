using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PSMoving", menuName = "Player State/Moving")]
public class PSMoving : PlayerState
{
    [SerializeField] private int index_PSIdle;


    public override int Update()
    {
        float x = Input.GetAxis("Horizontal");

        if (x * Input.GetAxis("Vertical") == 0)
            return index_PSIdle;

        Vector2 direction = x > 0 ? Vector2.left : Vector2.right;

        playerCharacter.GetComponent<Rigidbody2D>().velocity = direction * 10;

        return Index;
    }


    public override void OnStateEnter()
    {
        float x = Input.GetAxis("Horizontal");

        Vector2 direction = x > 0 ? Vector2.left : Vector2.right;

        playerCharacter.GetComponent<Rigidbody2D>().velocity = direction * 10;
    }
}
