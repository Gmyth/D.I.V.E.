using UnityEngine;


public abstract class EnemyState : State
{
    public static PlayerCharacter FindAvailableTarget(Vector3 enemyPosition, float range)
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


    public static PlayerCharacter FindAvailableTarget(Vector3 enemyPosition, float range, Zone guardZone)
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


    public override void OnStateEnter(State previousState) { }
    //public virtual void OnStateReset() { }
    public override void OnStateQuit(State nextState) { }
}


public abstract class EnemyState<T> : EnemyState where T : Enemy
{
    protected T enemy;


    public override void Initialize(int index, Enemy enemy)
    {
        base.Initialize(index, enemy);

        Initialize((T)enemy);
    }


    public virtual void Initialize(T enemy)
    {
        this.enemy = enemy;
    }


    protected PlayerCharacter IsPlayerInSight(float range)
    {
        return IsPlayerInSight(PlayerCharacter.Singleton, range);
    }


    protected PlayerCharacter IsPlayerInSight(PlayerCharacter player, float range)
    {
        if (!player)
            return null;


        RaycastHit2D raycastHit2D = Physics2D.Raycast(enemy.transform.position, player.transform.position - enemy.transform.position, Vector2.Distance(enemy.transform.position, player.transform.position), 1 << LayerMask.NameToLayer("Player"));

        if (raycastHit2D.collider && raycastHit2D.collider.gameObject != player.gameObject)
            return null;


        return player;
    }

    protected Vector2 GetGroundNormal()
    {
        RaycastHit2D hit = Physics2D.Raycast(enemy.transform.position, Vector2.down, 3f);

        if (hit.collider && hit.transform.CompareTag("Ground"))
            return hit.normal;

        return Vector2.zero;
    }

    protected void AdjustFacingDirection(Vector3 direction)
    {
        Vector3 scale = enemy.transform.localScale;
        scale.x = Mathf.Sign(direction.x) * Mathf.Abs(scale.x);

        enemy.transform.localScale = scale;
    }
}


[CreateAssetMenuAttribute(fileName = "FSM_Enemy", menuName = "State Machine/Enemy")]
public class FSMEnemy : FiniteStateMachine<EnemyState>
{
    public override int CurrentStateIndex
    {
        get
        {
            return currentStateIndex;
        }

        set
        {
            if (value != currentStateIndex)
            //{
#if UNITY_EDITOR
            //    Debug.Log(LogUtility.MakeLogStringFormat(GetType().Name, "Reset on state {0}", currentStateIndex));
#endif
            //    CurrentState.OnStateReset();
            //}
            //else
            {
#if UNITY_EDITOR
                Debug.Log(LogUtility.MakeLogStringFormat(GetType().Name, "Make transition from state {0} to {1}", states[currentStateIndex].name, states[value].name));
#endif

                int previousStateIndex = currentStateIndex;

                CurrentState.OnStateQuit(states[value]);

                currentStateIndex = value;

                CurrentState.OnStateEnter(states[previousStateIndex]);

                OnCurrentStateChange.Invoke(currentStateIndex, previousStateIndex);
            }
        }
    }


    public FSMEnemy Initialize(Enemy enemy)
    {
        FSMEnemy fsmCopy = Instantiate(this);

        for (int i = 0; i < states.Length; ++i)
        {
            EnemyState stateCopy = Instantiate(states[i]);
            stateCopy.Initialize(i, enemy);

            fsmCopy.states[i] = stateCopy;
        }

        fsmCopy.currentStateIndex = -1;


        return fsmCopy;
    }

    public override void Boot()
    {
        OnMachineBoot();

        currentStateIndex = startingStateIndex;

        CurrentState.OnStateEnter(CurrentState);

        OnCurrentStateChange.Invoke(currentStateIndex, -1);
    }

    public override void ShutDown()
    {
        int previousStateIndex = currentStateIndex;

        CurrentState.OnStateQuit(null);

        currentStateIndex = -1;

        OnMachineShutDown();

        OnCurrentStateChange.Invoke(-1, previousStateIndex);
    }

    public void Update()
    {
        if (currentStateIndex >= 0)
            CurrentStateIndex = CurrentState.Update();
    }
}
