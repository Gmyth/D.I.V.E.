using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.Playables;
// ReSharper disable All


public class DialogueManager: Singleton<DialogueManager>
{
    private TextDict[] Dict;
    private DialogueData[] dialogues;

    public UnityEvent DialogueEnd;
    public UnityEvent KeyPressed;

    private TimelineManager currentTimelineManager;

    public enum DialogueType
    {
        Normal = 0,
        Option,
        Unknown
    }
    public void InitDialogue()
    {
        string path = Application.dataPath + "/StreamingAssets" + "/Dialogue.json";
        string jsonString = File.ReadAllText(path);
        string fixedJsonString = JsonHelper.fixJson(jsonString);
        dialogues = JsonHelper.FromJson<DialogueData>(fixedJsonString);
    }

    public void InitUIText()
    {
        string path = Application.dataPath + "/StreamingAssets" + "/Dictionary.json";
        string jsonString = File.ReadAllText(path);
        string fixedJsonString = JsonHelper.fixJson(jsonString);
        Dict = JsonHelper.FromJson<TextDict>(fixedJsonString);
    }

    public string GetDialogueText(int index)
    {
        return dialogues[index].Text;
    }

    public DialogueData GetDialogueData(int index)
    {
        return dialogues[index];
    }
    private IEnumerator waitForKeyPress(KeyCode key, GUIDialogue win = null)
    {
        bool done = false;
        float coolDown = 0f;
        while (!done) // essentially a "while true", but with a bool to break out naturally
        {
            if (win  != null){
                if (coolDown >= 1 && Input.GetKeyDown(key) && win.CheckAnimateCoroutine() == false)
                {
                    done = true; // breaks the loop
                    coolDown = 0f;
                }
                yield return null; // wait until next frame, then continue execution from here (loop continues)
            }
            else{
                if (coolDown >= 1 && Input.GetKeyDown(key))
                {
                    done = true; // breaks the loop
                    coolDown = 0f;
                }
                yield return null; // wait until next frame, then continue execution from here (loop continues)
            }
            coolDown += Time.deltaTime;
        }

        // now this function returns
    }

    public void SetTimelineManager(TimelineManager timelineManager) 
    {
        currentTimelineManager = timelineManager;
    }

    /// <summary>
    /// Play a sequence of dialogue
    /// </summary>
    /// <param name="index"> The start id of dialogues </param>
    /// <param name="transform"> The actor's transform </param>
    /// <returns></returns>
    public IEnumerator PlayDialogue(int index, Transform transform)
    {
        GUIDialogue dialogueWin = (GUIDialogue)GUIManager.Singleton.Open("DialogueUI");

        dialogueWin.CurrentTimelineManager = currentTimelineManager;
        
        if (CheckDialogueType(index) == DialogueType.Normal)
            dialogueWin.DisplayDialogue(dialogues[index].Text, dialogues[index].Actor, transform);
        else if(CheckDialogueType(index) == DialogueType.Option)
        {
            
        }
        else
            yield break;

        yield return null;

        while(dialogues[index].Next != "-1")
        {
            Debug.Log("Enter Loop");
            yield return waitForKeyPress(KeyCode.G);
            if (dialogueWin.CheckAnimateCoroutine())
            {
                dialogueWin.SetSkip(true);        
                yield return waitForKeyPress(KeyCode.G, dialogueWin);
                dialogueWin.SetSkip(false);
            }

            int nextDialogue = Convert.ToInt32(dialogues[index].Next);

            if (CheckDialogueType(index) == DialogueType.Normal)
                dialogueWin.DisplayDialogue(dialogues[nextDialogue].Text, dialogues[nextDialogue].Actor, transform);

            index = nextDialogue;
        }
        yield return waitForKeyPress(KeyCode.G, dialogueWin);
        GUIManager.Singleton.Close("DialogueUI");
        DialogueEnd.Invoke();      
    }
    private DialogueType CheckDialogueType(int index)
    {
        if (dialogues[index].Type == "N" || dialogues[index].Type == "Normal")
            return DialogueType.Normal;
        else if(dialogues[index].Type == "O" || dialogues[index].Type == "Option")
            return DialogueType.Option;
        else
            return DialogueType.Unknown;
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
    public string Next;
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

