using System;
using UnityEngine;

[Serializable] public class SkillData : DataTableEntry
{
    [SerializeField] private int id;
    [SerializeField] private string name;
    [SerializeField] private Sprite icon;
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

    public Sprite Icon
    {
        get
        {
            return icon;
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


[CreateAssetMenuAttribute(fileName = "Skill", menuName = "Data Table/Skill")]
public class SkillDataTable : DataTable<SkillData> { }
