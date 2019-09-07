using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class EnemyState : State
{
    protected Dummy dummy;



    public virtual void Initialize(int index, Dummy dummy)
    {
        Index = index;
        this.dummy = dummy;
    }


    public abstract int Update();


    public virtual void OnStateEnter() { }
    //public virtual void OnStateReset() { }
    public virtual void OnStateQuit() { }
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
            //    Debug.Log(LogUtility.MakeLogStringFormat("FSMPLayer", "Reset on state {0}", currentStateIndex));

            //    CurrentState.OnStateReset();
            //}
            //else
            {
                Debug.Log(LogUtility.MakeLogStringFormat("FSMEnemy", "Make transition from state {0} to {1}", currentStateIndex, value));

                int previousStateIndex = currentStateIndex;

                CurrentState.OnStateQuit();

                currentStateIndex = value;

                CurrentState.OnStateEnter();

                OnCurrentStateChange.Invoke(currentStateIndex, previousStateIndex);
            }
        }
    }


    public void Initialize(Dummy dummy)
    {
        for (int i = 0; i < states.Length; ++i)
            states[i].Initialize(i, dummy);

        currentStateIndex = -1;
    }

    public override void Boot()
    {
        OnMachineBoot();

        currentStateIndex = startingStateIndex;

        CurrentState.OnStateEnter();

        OnCurrentStateChange.Invoke(currentStateIndex, -1);
    }

    public override void ShutDown()
    {
        int previousStateIndex = currentStateIndex;

        CurrentState.OnStateQuit();

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
