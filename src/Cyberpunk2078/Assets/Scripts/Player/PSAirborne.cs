using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Airborne", menuName = "Player State/Airborne")]
public class PSAirborne : PlayerState
{
    [SerializeField] private int index_PSIdle;
    [SerializeField] private int index_PSMoving;


    public override int Update()
    {
        Transform transform = playerCharacter.transform;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0f, -0.2f, 0f), -transform.up, 0.5f);
        RaycastHit2D hit1 = Physics2D.Raycast(transform.position + new Vector3(0.1f, -0.2f, 0f), -transform.up, 0.5f);
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position + new Vector3(-0.1f, -0.2f, 0f), -transform.up, 0.5f);

        if (hit.collider && hit.transform.CompareTag("Ground") || hit1.collider && hit1.transform.CompareTag("Ground") || hit2.collider && hit2.transform.CompareTag("Ground"))
            return Input.GetAxis("Horizontal") * Input.GetAxis("Vertical") == 0 ? index_PSIdle : index_PSMoving;

        return Index;
    }
}
