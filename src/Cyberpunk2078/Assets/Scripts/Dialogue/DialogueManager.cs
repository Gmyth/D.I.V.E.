using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;


public class DialogueManager: Singleton<DialogueManager>
{
    private TextDict[] Dict;
    private DialogueData[] dialogues;

    public void InitDialogue()
    {
        string path = Application.dataPath + "/StreamingAssets" + "/Dialogue.json";
        string jsonString = File.ReadAllText(path);
        string fixedJsonString = JsonHelper.fixJson(jsonString);
        dialogues = JsonHelper.FromJson<DialogueData>(fixedJsonString);
        Debug.Log(dialogues[0].Actor);
    }

    public void InitUIText()
    {
        string path = Application.dataPath + "/StreamingAssets" + "/Dictionary.json";
        string jsonString = File.ReadAllText(path);
        string fixedJsonString = JsonHelper.fixJson(jsonString);
        Dict = JsonHelper.FromJson<TextDict>(fixedJsonString);
        Debug.Log(Dict[11].Text);
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

[Serializable]
public class TextDict
{
    public int ID;
    public string Text;
    public string Type;
}

