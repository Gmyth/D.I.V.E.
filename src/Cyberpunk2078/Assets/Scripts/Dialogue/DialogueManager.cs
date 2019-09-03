using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;


public class DialogueManager: Singleton<DialogueManager>
{   
    private DialogueData[] dialogues;

    public void InitDialogue()
    {
        string path = Application.dataPath + "/StreamingAssets" + "/Dia.json";
        string jsonString = File.ReadAllText(path);
        string fixedJsonString = JsonHelper.fixJson(jsonString);
        dialogues = JsonHelper.FromJson<DialogueData>(fixedJsonString);
        Debug.Log(dialogues);
    }
}

[Serializable]
public class DialogueData
{
    public int ID;
    public string Type;
    public string Actor;
    public string Image;
    public string Text;
    public string Sound;
    public string Effect;
    public string Chapter;
    public string Scene;

}

