using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIDialogue : GUIWindow
{
    Dictionary<string, GameObject> ActorDict = new Dictionary<string, GameObject>();
    List<GameObject> TextBoxes = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void SetText(string text, string actor)
    {
        if(!ActorDict.ContainsKey(actor))
        {
            GameObject TextBox = Instantiate(ResourceUtility.GetGUIPrefab<GameObject>("TextBox"), transform, false);
            TextBoxes
            ActorDict.Add(actor, TextBox);
        }



        if(TextBoxA.GetComponent<Text>().text == null)
        {
            ActorDict.Add(TextBoxA, actor);
            TextBoxA.GetComponent<Text>().text = text;
            return;
        }
        else if(TextBoxB.GetComponent<Text>().text == null)
        {
            ActorDict.Add(TextBoxB, actor);
            TextBoxB.GetComponent<Text>().text = text;
            return;
        }
        else
        {
            Debug.Log(LogUtility.MakeLogStringFormat("GUIDialogue", "TextBoxesOccupied"));
            return;
        }
    }
}

