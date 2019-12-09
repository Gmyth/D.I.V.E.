using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GUIDialogueWidget : GUIWidget
{
    private static string StripAllCommands(string text)
    {
        //Regex Pattern. Remove all "{stuff:value}" from our dialogue line.
        const string pattern = "\\{.[^}]+\\}";

        //Clean string to return.
        string cleanString = Regex.Replace(text, pattern, "");


        return cleanString;
    }


    public float textSpeed = 0.1f;
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI textField;

    DialogueData dialogue = null;

    int i0;


    public void Show(DialogueData dialogue)
    {
        ShowDialogue(dialogue);


        Show();
    }


    private void ShowDialogue(DialogueData dialogue)
    {
        this.dialogue = dialogue;


        portrait.sprite = dialogue.Portrait;


        DataTableManager dataTableManager = DataTableManager.singleton;

        string speakerName = dataTableManager.GetText(dialogue.TextID_Speaker);
        string content = dataTableManager.GetText(dialogue.TextID_Content);


        textField.text = StripAllCommands(speakerName + ":\n" + content);

        //We now hide text based on each character's alpha value
        HideText(speakerName.Length + 2);


        i0 = speakerName.Length + 2;
    }


    private void Start()
    {
        StartCoroutine(ShowText());
    }


    private IEnumerator ShowText()
    {
        for (;;)
        {
            //Count how many characters we have in our new dialogue line.
            int numCharacters = textField.text.Length;


            float t = Time.unscaledTime;
            
            while (textField.maxVisibleCharacters < numCharacters)
            {
                float dt = Time.unscaledTime - t;
                float ts = Input.GetButton("Attack1") ? 0.2f * textSpeed : textSpeed;
                int n = Mathf.FloorToInt(dt / ts);

                textField.maxVisibleCharacters += n;
                t += n * ts;

                yield return null;
            }


            yield return WaitForKeyPress("Attack1");


            if (dialogue.Next < 0)
                break;
            else
                ShowDialogue(DataTableManager.singleton.GetDialogueData(dialogue.Next));
        }


        Hide();
    }

    private IEnumerator WaitForKeyPress(string buttonName, float maxDuration = float.MaxValue)
    {
        float t = Time.unscaledTime;

        for (; Time.unscaledTime - t < maxDuration && !Input.GetButtonDown(buttonName);)
        {
            yield return null;
        }
    }

    private void HideText(int numToShow = 0)
    {
        textField.firstVisibleCharacter = 0;
        textField.maxVisibleCharacters = numToShow;
        textField.ForceMeshUpdate();
    }
}
