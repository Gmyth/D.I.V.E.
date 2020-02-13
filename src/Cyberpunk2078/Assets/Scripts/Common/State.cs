using UnityEngine;


public abstract class State : ScriptableObject
{
    [SerializeField] private string stateName = "";


    public int Index { get; protected set; } = -1;
    public string Name
    {
        get
        {
            return stateName;
        }
    }


    public abstract string Update();
    

    public virtual void OnStateEnter(State previousState) { }
    //public abstract void OnStateReset();
    public virtual void OnStateQuit(State nextState) { }


    public virtual void OnMachineBoot() { }
    public virtual void OnMachineShutDown() { }
}
