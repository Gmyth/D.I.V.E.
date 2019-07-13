using UnityEngine;


public abstract class FiniteStateMachine<T> : ScriptableObject where T : State
{
    [SerializeField] protected T[] states;
    [SerializeField] protected int startingState;

    protected int currentStateIndex;


    public Event2<int> OnCurrentStateChange { get; } = new Event2<int>();

    public virtual int CurrentStateIndex
    {
        get
        {
            return currentStateIndex;
        }

        set
        {
            currentStateIndex = value;
        }
    }

    public T CurrentState
    {
        get
        {
            return states[currentStateIndex];
        }
    }


    public virtual void OnMachineStart()
    {
        CurrentStateIndex = startingState;
    }

    public virtual void OnMachineStop() { }
}
