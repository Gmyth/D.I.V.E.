using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    string path;
    string jsonString;
    public DialogueData dialogues;
    // Use this for initialization
    void Start()
    {
        path = Application.dataPath + "/StreamingAssets" + "/test.json";
        jsonString = File.ReadAllText(path);
        dialogues = DialogueData.CreateFromJSON(jsonString);
        print(dialogues);


    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(DialogueController.singleton.Dialogue != dialogues)
                DialogueController.singleton.SetDialogue(dialogues);           
            DialogueController.singleton.TriggerDialogue();
        }
    }


}

public class DialogueController: MonoBehaviour
{
    public static readonly DialogueController singleton = new DialogueController();

   

    public DialogueData Dialogue { get; set; }
    private int next_cue = 0;
    GameObject dialogue_1 = null;
    GameObject dialogue_2 = null;

    public void TriggerDialogue()
    {

        Transform canvasTransform = GameObject.Find("Canvas").transform;
        if(next_cue == 0)
        {
            dialogue_1 = Instantiate(Resources.Load<GameObject>("Dialogue1"), canvasTransform, false);
            dialogue_2 = Instantiate(Resources.Load<GameObject>("Dialogue2"), canvasTransform, false);
        }
        
        if (next_cue == -2)
        {
            dialogue_1.GetComponent<GUIWidget>().Hide();
            dialogue_2.GetComponent<GUIWidget>().Hide();
            next_cue = 0;
            Destroy(dialogue_1);
            Destroy(dialogue_2);
            return;
        }
        if (next_cue % 2 == 0)
        {
            int actor = Dialogue.conversations[0].cues[next_cue].actor;
            dialogue_1.GetComponent<Text>().text = Dialogue.actors[actor-1].name + ":" + Dialogue.conversations[0].cues[next_cue].text;

        }
        else
        {
            int actor = Dialogue.conversations[0].cues[next_cue].actor;
            dialogue_2.GetComponent<Text>().text = Dialogue.actors[actor-1].name + ":" + Dialogue.conversations[0].cues[next_cue].text; 
        }

        next_cue = Dialogue.conversations[0].cues[next_cue].next_cue - 1;      
    }

    public void SetDialogue(DialogueData dialogue)
    {
        Dialogue = dialogue;
    }
}