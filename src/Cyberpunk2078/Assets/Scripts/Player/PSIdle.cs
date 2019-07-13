using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PSIdle", menuName = "Player State/Idle")]
public class PSIdle : PlayerState
{
    [SerializeField] private int index_PSMoving;


    public override int Update()
    {
        if (Input.GetAxis("Horizontal") * Input.GetAxis("Vertical") != 0)
            return index_PSMoving;

        return Index;
    }
}
