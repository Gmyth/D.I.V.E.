using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenuAttribute(fileName = "PS_Knockback", menuName = "Player State/Knockback")]
public class PSKnockback : PlayerState
{
    [SerializeField] public float duration;

    [Header("Connected States")]
    [SerializeField] private int stateIndex_Idle;
    [SerializeField] private int stateIndex_Moving;
    [SerializeField] private int stateIndex_jumping;
    [SerializeField] private int stateIndex_Dashing;

    [HideInInspector] public Vector3 knockback;
    
    private Rigidbody2D rigidbody;

    private float t;


    public override void Initialize(int index, PlayerCharacter playerCharacter)
    {
        base.Initialize(index, playerCharacter);


        rigidbody = playerCharacter.GetComponent<Rigidbody2D>();
    }

    public override int Update()
    {
        if (Time.time >= t)
            return stateIndex_Idle;


        return Index;
    }

    public override void OnStateEnter(State previousState)
    {
        t = Time.time + duration;
    }
}
