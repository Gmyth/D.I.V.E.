using UnityEngine;


public abstract class FiniteStateMachine<T> : ScriptableObject where T : State
{
    [SerializeField] protected T[] states;
    [SerializeField] protected int startingStateIndex;

    protected int currentStateIndex = -1;


    public Event2<int> OnCurrentStateChange { get; } = new Event2<int>();

    public T this[int index]
    {
        get
        {
            return states[index];
        }
    }

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
            return currentStateIndex >= 0 ? states[currentStateIndex] : null;
        }
    }


    public abstract void Boot();

    public abstract void ShutDown();


    public virtual void OnMachineBoot() { }
    public virtual void OnMachineShutDown() { }
}
