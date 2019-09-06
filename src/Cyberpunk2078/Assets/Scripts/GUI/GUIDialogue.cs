using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GUIDialogueMode
{
    Default = 0,
    Multiple
}

public class GUIDialogue : GUIWindow
{
    private Dictionary<string, GameObject> TextBoxDict = new Dictionary<string, GameObject>();

    private List<GameObject> TextBoxList = new List<GameObject>();

    private GUIDialogueMode mode;

    private Coroutine animateCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        mode = GUIDialogueMode.Default;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void CheckActor(string actor)
    {
        if(mode == GUIDialogueMode.Default)
        {
            if (TextBoxList.Count == 0)
            {
                GameObject TextBox = Instantiate(ResourceUtility.GetGUIPrefab<GameObject>("TextBox"), transform, false);
                
                TextBoxList.Add(TextBox);
            }
            if(!TextBoxDict.ContainsKey(actor))
                TextBoxDict.Add(actor, TextBoxList[0]);
        }
        else if(mode == GUIDialogueMode.Multiple)
        {
        }  
    }
    private void AssignTextBoxPos(string actor, Transform transform)
    {
        Vector2 actorCoordinates = Camera.main.WorldToScreenPoint(transform.position);

        actorCoordinates.y += 40;

        TextBoxDict[actor].GetComponent<RectTransform>().position = actorCoordinates;
    }

    public void SetText(string text, string actor)
    {
        CheckActor(actor);

        TextBoxDict[actor].GetComponent<Text>().text = actor + ":" + text;
    }

    public void SetText(string text, string actor, Transform transform)
    {
        CheckActor(actor);

        AssignTextBoxPos(actor, transform);

        if(animateCoroutine != null)
            StopCoroutine(animateCoroutine);
        animateCoroutine = StartCoroutine(AnimateTextCoroutine(actor, actor + ":" + text));

        //TextBoxDict[actor].GetComponent<Text>().text = actor + ":" + text;
    }

    private IEnumerator AnimateTextCoroutine(string actor, string text)
    {

        //Reset Dialogue Box.
        TextBoxDict[actor].GetComponent<Text>().text = "";
        int i = 0;

        //Loop over the string.
        while (i < text.Length)
        {

            //Add a character to the dialogue text field.
            TextBoxDict[actor].GetComponent<Text>().text += text[i];

            i++;    //increment

            //Wait before animating next character in scenarioText.
            yield return new WaitForSeconds(0.05f);
        }

        Debug.Log("Done animating!");
    }
}

