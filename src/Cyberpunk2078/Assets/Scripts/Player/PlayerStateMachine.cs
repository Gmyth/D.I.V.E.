public abstract class PlayerState : State
{
    protected PlayerCharacter playerCharacter;


    public virtual void Initialize(int index, PlayerCharacter playerCharacter)
    {
        Index = index;
        this.playerCharacter = playerCharacter;
    }


    public abstract int Update();


    public virtual void OnStateEnter() { }
    public virtual void OnStateReset() { }
    public virtual void OnStateQuit() { }
}


public class PlayerStateMachine : FiniteStateMachine<PlayerState>
{
    public override int CurrentStateIndex
    {
        get
        {
            return currentStateIndex;
        }

        set
        {
            if (value == currentStateIndex)
                CurrentState.OnStateReset();
            else
            {
                int previousStateIndex = currentStateIndex;

                CurrentState.OnStateQuit();

                currentStateIndex = value;

                CurrentState.OnStateEnter();

                OnCurrentStateChange.Invoke(currentStateIndex, previousStateIndex);
            }
        }
    }


    public PlayerStateMachine(PlayerCharacter player)
    {
        for (int i = 0; i < states.Length; ++i)
            states[i].Initialize(i, player);
    }


    public void Update()
    {
        CurrentStateIndex = CurrentState.Update();
    }
}
