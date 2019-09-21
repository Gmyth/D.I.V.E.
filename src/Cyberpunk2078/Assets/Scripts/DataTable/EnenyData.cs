using System;
using UnityEngine;


[Serializable] public class EnemyData : DataTableEntry
{
    [SerializeField] private int id;
    [SerializeField] private string name;
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


[CreateAssetMenuAttribute(fileName = "Enemy", menuName = "Data Table/Enemy")]
public class EnemyDataTable : DataTable<EnemyData> { }