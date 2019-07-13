using UnityEngine;


public abstract class State : ScriptableObject
{
    public int Index { get; protected set; } = -1;
}
