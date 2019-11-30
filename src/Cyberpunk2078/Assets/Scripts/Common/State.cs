using UnityEngine;


public abstract class State : ScriptableObject
{
    public int Index { get; protected set; } = -1;


    public abstract int Update();


    public virtual void OnStateEnter(State previousState) { }
    //public abstract void OnStateReset();
    public virtual void OnStateQuit(State nextState) { }


    public virtual void OnMachineBoot() { }
    public virtual void OnMachineShutDown() { }
}
