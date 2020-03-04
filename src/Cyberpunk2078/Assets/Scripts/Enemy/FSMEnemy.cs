﻿using UnityEngine;


public abstract class EnemyState : State
{
    protected static PlayerCharacter IsPlayerInSight(PlayerCharacter player, Enemy enemy, float range)
    {
        if (!player)
            return null;


        /* Check whether player is in the sight range */
        float d = Vector2.Distance(enemy.transform.position, player.transform.position);


        if (range > 0 && d > range)
            return null;


        /* Check whether there is something in the way */
        int collidedLayer = (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("Obstacle"));

        RaycastHit2D raycastHit2D = Physics2D.Raycast(enemy.transform.position, player.transform.position - enemy.transform.position, d, collidedLayer);

        if (raycastHit2D.collider && raycastHit2D.collider.gameObject != player.gameObject)
            return null;


        return player;
    }

    protected static PlayerCharacter FindAvailableTarget(Vector3 enemyPosition, float range)
    {
        PlayerCharacter player = PlayerCharacter.Singleton;

        Vector3 playerPosition = player.transform.position;


        float d = Vector2.Distance(enemyPosition, playerPosition);

        if (d <= range)
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(enemyPosition, playerPosition - enemyPosition, d, 1 << LayerMask.NameToLayer("Player"));

            if (raycastHit2D.collider && raycastHit2D.collider.gameObject == player.gameObject)
                return player;
        }


        return null;
    }


    protected static PlayerCharacter FindAvailableTarget(Vector3 enemyPosition, float range, Zone guardZone)
    {
        PlayerCharacter player = PlayerCharacter.Singleton;

        Vector3 playerPosition = player.transform.position;


        float d = Vector2.Distance(enemyPosition, playerPosition);

        if (d <= range && guardZone.Contains(playerPosition))
        {
            RaycastHit2D raycastHit2D = Physics2D.Raycast(enemyPosition, playerPosition - enemyPosition, d, 1 << LayerMask.NameToLayer("Player"));

            if (raycastHit2D.collider == null || raycastHit2D.collider.gameObject == player.gameObject)
                return player;
        }


        return null;
    }


    public virtual void Initialize(int index, Enemy enemy)
    {
        Index = index;
    }
}


public abstract class EnemyState<T> : EnemyState where T : Enemy
{
    protected T enemy;
    protected Rigidbody2D enemyRigidbody;
    protected Animator enemyAnimator;


    public override void Initialize(int index, Enemy enemy)
    {
        base.Initialize(index, enemy);

        Initialize((T)enemy);
    }


    public virtual void Initialize(T enemy)
    {
        this.enemy = enemy;
        enemyRigidbody = enemy.GetComponent<Rigidbody2D>();
        enemyAnimator = enemy.GetComponent<Animator>();
    }


    protected PlayerCharacter IsPlayerInSight(float range)
    {
        return IsPlayerInSight(PlayerCharacter.Singleton, range);
    }

    protected PlayerCharacter IsPlayerInSight(PlayerCharacter player, float range)
    {
        return IsPlayerInSight(player, enemy, range);
    }


    protected Vector2 GetGroundNormal()
    {
        int layerMask = LayerMask.GetMask("Obstacle");


        RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position + 0.1f * Vector3.up, Vector2.down, 0.5f, layerMask);

        if (hit.collider && hit.transform.CompareTag("Ground"))
            return hit.normal;


        return Vector2.zero;
    }


    protected PlayerCharacter FindAvailableTarget(bool useSightRange = true, bool useGuardZone = true)
    {
        if (useSightRange)
            return useGuardZone ? FindAvailableTarget(enemy.transform.position, enemy[StatisticType.SightRange], enemy.GuardZone) : FindAvailableTarget(enemy.transform.position, enemy[StatisticType.SightRange]);


        return null;
    }
}


[CreateAssetMenuAttribute(fileName = "FSM_Enemy", menuName = "State Machine/Enemy")]
public class FSMEnemy : FiniteStateMachine<EnemyState>
{
    public override string CurrentStateName
    {
        get
        {
            return currentStateName;
        }

        set
        {
            if (value != currentStateName)
            //{
#if UNITY_EDITOR
            //    Debug.Log(LogUtility.MakeLogStringFormat(GetType().Name, "Reset on state {0}", currentStateIndex));
#endif
            //    CurrentState.OnStateReset();
            //}
            //else
            {
#if UNITY_EDITOR
                Debug.Log(LogUtility.MakeLogStringFormat(GetType().Name, "Make transition from state {0} to {1}", currentStateName, value));
#endif

                string previousStateName = currentStateName;
                EnemyState previousState = states[previousStateName];


                CurrentState.OnStateQuit(states[value]);

                currentStateName = value;

                CurrentState.OnStateEnter(previousState);

                OnCurrentStateChange.Invoke(states[currentStateName], previousState);
            }
        }
    }


    public FSMEnemy Initialize(Enemy enemy)
    {
        FSMEnemy fsmCopy = Instantiate(this);

        for (int i = 0; i < serializedStates.Length; ++i)
        {
            EnemyState stateCopy = Instantiate(serializedStates[i]);
            stateCopy.Initialize(i, enemy);

            fsmCopy.states.Add(stateCopy.Name, stateCopy);
        }

        fsmCopy.currentStateName = "";


        return fsmCopy;
    }

    public override void Boot()
    {
        OnMachineBoot();

        currentStateName = startingState;

        CurrentState.OnStateEnter(null);

        OnCurrentStateChange.Invoke(states[currentStateName], null);
    }

    public override void Reboot()
    {
        if (IsRunning)
            ShutDown();

        Boot();
    }

    public override void ShutDown()
    {
        string previousStateName = currentStateName;

        CurrentState.OnStateQuit(null);

        currentStateName = "";

        OnMachineShutDown();

        OnCurrentStateChange.Invoke(null, states[previousStateName]);
    }

    public void Update()
    {
        if (currentStateName != "")
            CurrentStateName = CurrentState.Update();
    }
}
