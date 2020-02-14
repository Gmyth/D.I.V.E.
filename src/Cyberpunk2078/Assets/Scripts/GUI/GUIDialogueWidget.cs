﻿using System;
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


    public float textSpeed = 1f;
    [SerializeField] private Image portrait;
    [SerializeField] private TextMeshProUGUI textField;

    DialogueData dialogue = null;
    private Action[] currentCallbacks = null;
    
    
    public void Show(DialogueData dialogue, params Action[] callbacks)
    {
        currentCallbacks = callbacks;


        ShowDialogue(dialogue);


        Show();
    }
    
    public override void Hide()
    {
        base.Hide();

        StopAllCoroutines();
        
        currentCallbacks = null;
    }


    private void ShowDialogue(DialogueData dialogue)
    {
        this.dialogue = dialogue;


        portrait.sprite = dialogue.Portrait;


        DataTableManager dataTableManager = DataTableManager.singleton;

        string speakerName = dataTableManager.GetText(dialogue.TextID_Speaker);
        string content = dataTableManager.GetText(dialogue.TextID_Content);


        textField.text = StripAllCommands(speakerName + "\n" + content);

        //We now hide text based on each character's alpha value
        HideText(speakerName.Length + 1);
    }


    private void OnEnable()
    {
        StartCoroutine(ShowText(currentCallbacks));
    }


    private IEnumerator ShowText(params Action[] callbacks)
    {
        for (;;)
        {
            int numCharacters = textField.text.Length;
            float t = Time.unscaledTime;

            while (textField.maxVisibleCharacters < numCharacters)
            {
                if (Input.GetButtonDown("Attack1"))
                {
                    AudioManager.Singleton.PlayOnce("Dialogue_next");
                    break;
                }

                AudioManager.Singleton.PlayEvent("Dialogue_printing");


                float dt = Time.unscaledTime - t;
                float ts = Input.GetButton("Dashing") ? 0.2f * textSpeed : textSpeed;
                int n = Mathf.FloorToInt(dt / ts);

                textField.maxVisibleCharacters += n;
                t += n * ts;


                yield return null;
            }

            AudioManager.Singleton.StopEvent("Dialogue_printing");

            textField.maxVisibleCharacters = numCharacters;

            yield return WaitForKeyPress("Attack1");

            AudioManager.Singleton.PlayOnce("Dialogue_next");


            if (dialogue.Next < 0)
                break;
            else
                ShowDialogue(DataTableManager.singleton.GetDialogueData(dialogue.Next));
        }


        if (callbacks != null)
            foreach (Action callback in callbacks)
                callback?.Invoke();
    }

    private IEnumerator WaitForKeyPress(string buttonName, float maxDuration = float.MaxValue)
    {
        yield return null;


        float t = Time.unscaledTime;

        for (; Time.unscaledTime - t < maxDuration && !Input.GetButtonDown(buttonName);)
            yield return null;


        yield return null;
    }

    private void HideText(int numToShow = 0)
    {
        textField.firstVisibleCharacter = 0;
        textField.maxVisibleCharacters = numToShow;
        textField.ForceMeshUpdate();
    }
}
