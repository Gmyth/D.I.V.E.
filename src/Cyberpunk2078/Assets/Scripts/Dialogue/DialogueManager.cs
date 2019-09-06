using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.Events;


public class DialogueManager: Singleton<DialogueManager>
{
    private TextDict[] Dict;
    private DialogueData[] dialogues;

    public UnityEvent DialogueEnd;

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
    private IEnumerator waitForKeyPress(KeyCode key)
    {
        bool done = false;
        while (!done) // essentially a "while true", but with a bool to break out naturally
        {
            if (Input.GetKeyDown(key))
            {
                done = true; // breaks the loop
            }
            yield return null; // wait until next frame, then continue execution from here (loop continues)
        }

        // now this function returns
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
        dialogueWin.SetText(dialogues[index].Text, dialogues[index].Actor, transform);
        yield return null;
        while(dialogues[index].Next != "-1")
        {
            Debug.Log("Enter Loop");
            yield return waitForKeyPress(KeyCode.G);
            int nextDialogue = Convert.ToInt32(dialogues[index].Next);
            dialogueWin.SetText(dialogues[nextDialogue].Text, dialogues[nextDialogue].Actor, transform);
            index = nextDialogue;
        }
        yield return waitForKeyPress(KeyCode.G);
        GUIManager.Singleton.Close("DialogueUI");
        DialogueEnd.Invoke();      
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

