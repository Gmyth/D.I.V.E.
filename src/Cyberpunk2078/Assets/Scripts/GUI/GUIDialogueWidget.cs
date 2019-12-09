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
        this.dialogue = dialogue;


        portrait.sprite = dialogue.Portrait;


        DataTableManager dataTableManager = DataTableManager.singleton;

        string speakerName = dataTableManager.GetText(dialogue.TextID_Speaker);
        string content = dataTableManager.GetText(dialogue.TextID_Content);


        textField.text = StripAllCommands(speakerName + ":\n" + content);

        //We now hide text based on each character's alpha value
        HideText(speakerName.Length + 2);


        i0 = speakerName.Length + 2;


        Show();
    }


    private void Start()
    {
        StartCoroutine(ShowText());
    }


    private IEnumerator ShowText()
    {
        textField.ForceMeshUpdate();


        //Count how many characters we have in our new dialogue line.
        TMP_TextInfo textInfo = textField.textInfo;
        int numCharacters = textInfo.characterCount;


        float t = Time.unscaledTime;
        
        while (textField.maxVisibleCharacters < numCharacters)
        {
            textField.maxVisibleCharacters = i0 + Mathf.FloorToInt((Time.unscaledTime - t) / textSpeed);

            yield return null;
        }
    }

    private IEnumerator WaitForKeyPress(KeyCode key, GUIDialogue win = null)
    {
        bool done = false;
        float coolDown = 1f;
        while (!done) // essentially a "while true", but with a bool to break out naturally
        {
            if (win != null)
            {
                if (coolDown >= 1 && Input.GetKeyDown(key) && win.CheckAnimateCoroutine() == false)
                {
                    done = true; // breaks the loop
                    coolDown = 0f;
                }
                yield return null; // wait until next frame, then continue execution from here (loop continues)
            }
            else
            {
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

    private void HideText(int numToShow = 0)
    {
        textField.firstVisibleCharacter = 0;
        textField.maxVisibleCharacters = numToShow;
    }
}
