using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable] public class DialogueData: DataTableEntry
{
    public int id;
    public Actors[] actors;
    public Conversations[] conversations;

    public static DialogueData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<DialogueData>(jsonString);
    }

    public override int Index
    {
        get
        {
            return id;
        }
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}

[Serializable] public struct Actors
{
    public int id;
    public string name;
}

[Serializable] public struct Cues
{
    public int id;
    public int actor;
    public int conversant;
    public string text;
    public int next_cue;
}

[Serializable] public struct Conversations
{
    public int id;
    public string title;
    public Cues[] cues;
}

[CreateAssetMenuAttribute(fileName = "Dialogue", menuName = "Data Table/Dialogue")]
public class DialogueDataTable : DataTable<DialogueData> { }