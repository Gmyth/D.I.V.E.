using System.Collections.Generic;
using UnityEngine;


public abstract class FiniteStateMachine<T> : ScriptableObject where T : State
{
    [SerializeField] protected T[] serializedStates;
    [SerializeField] protected string startingState;

    protected Dictionary<string, T> states = new Dictionary<string, T>();
    protected string currentStateName = "";


    public Event2<T> OnCurrentStateChange { get; } = new Event2<T>();

    public T this[string name]
    {
        get
        {
            return states[name];
        }
    }

    public bool IsRunning
    {
        get
        {
            return currentStateName != "";
        }
    }

    public virtual string CurrentStateName
    {
        get
        {
            return currentStateName;
        }

        set
        {
            currentStateName = value;
        }
    }

    public T CurrentState
    {
        get
        {
            return IsRunning ? states[currentStateName] : null;
        }
    }


    public abstract void Boot();
    public abstract void Reboot();
    public abstract void ShutDown();


    public virtual void OnMachineBoot() { }
    public virtual void OnMachineShutDown() { }
}
