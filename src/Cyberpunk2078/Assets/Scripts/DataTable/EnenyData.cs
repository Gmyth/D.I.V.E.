using System;
using UnityEngine;


public enum EnemyType
{
    Ground,
    Floating,
}


[Serializable] public class EnemyData : DataTableEntry
{
    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] private EnemyType type;
    [SerializeField] private AttributeSet attributes;


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

    public EnemyType Type
    {
        get
        {
            return type;
        }
    }

    public AttributeSet Attributes
    {
        get
        {
            return attributes;
        }
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}
