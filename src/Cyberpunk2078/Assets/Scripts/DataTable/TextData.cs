using System;
using UnityEngine;


public enum Language : int
{
    English = 0,
}


[Serializable] public class TextData : DataTableEntry
{
    [SerializeField] private int id;
    [SerializeField] private string english;


    public override int Index
    {
        get
        {
            return id;
        }
    }

    public int Id
    {
        get
        {
            return id;
        }
    }

    public string English
    {
        get
        {
            return english;
        }
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}
