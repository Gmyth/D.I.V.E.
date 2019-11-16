using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCharacter : Dummy
{
    public static PlayerCharacter Singleton { get; private set; } = null;


    [SerializeField] private FSMPlayer fsm;

    private StatisticSystem statistic;
    private new Rigidbody2D rigidbody;

    public GameObject dashAtkBox;

    public PlayerState State => fsm.CurrentState;


    public FSMPlayer GetFSM()
    {
        return fsm;
    }

    public PlayerCharacter(Player player)
    {
        statistic = new StatisticSystem(player.attributes, player.inventory);
    }

 
    public void Knockback(Vector3 origin, float force)
    {
        Vector3 direction = transform.position - origin;
        direction = (direction.x > 0 ? GroundNormal.Right().normalized : GroundNormal.Left().normalized) * force;
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(direction);


        fsm.CurrentStateIndex = 10;
    }


    public override float ApplyDamage(float rawDamage)
    {
        Debug.Log(LogUtility.MakeLogStringFormat(name, "Take {0} damage.", rawDamage));


        Player.CurrentPlayer.ApplyHealthChange(-rawDamage);


        return rawDamage;
    }
    

    public override void Dead()
    {
        //game over
    }


    private void Awake()
    {
        if (Singleton)
            Destroy(gameObject);
        else
            Singleton = this;
    }

    private void Start()
    {
        dashAtkBox = GetComponentInChildren<HitBox>()?.gameObject;
        if (!dashAtkBox)
            dashAtkBox = GetComponentInChildren<Attack>()?.gameObject;

        dashAtkBox.SetActive(false);


        rigidbody = GetComponent<Rigidbody2D>();


        fsm = fsm.Initialize(this);
        fsm.Boot();
    }

    private void Update()
    {
        fsm.Update();
    }

    private void OnDestroy()
    {
        if (Singleton == this)
            Singleton = null;
    }
}
