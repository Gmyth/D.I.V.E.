using System;
using UnityEngine;


[Serializable] public class DialogueData : DataTableEntry
{
    [SerializeField] private int id;
    [SerializeField] private Sprite portrait;
    [SerializeField] private int textID_speaker;
    [SerializeField] private int textID_content;
    [SerializeField] private int next;


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

    public int TextID_Speaker
    {
        get
        {
            return textID_speaker;
        }
    }

    public Sprite Portrait
    {
        get
        {
            return portrait;
        }
    }

    public int TextID_Content
    {
        get
        {
            return textID_content;
        }
    }

    public int Next
    {
        get
        {
            return next;
        }
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}