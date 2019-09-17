﻿using UnityEngine;


public abstract class State : ScriptableObject
{
    public int Index { get; protected set; } = -1;


    public abstract int Update();


    public abstract void OnStateEnter(State previousState);
    //public abstract void OnStateReset();
    public abstract void OnStateQuit(State nextState);
}
