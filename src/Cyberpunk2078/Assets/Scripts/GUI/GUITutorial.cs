using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GUITutorial : MonoBehaviour
{
    [SerializeField] private string keyboardIcons;
    [SerializeField] private string joystickIcons;
    [SerializeField] private GameObject directionalIcons;
    [SerializeField] private Image leftIcon;
    [SerializeField] private Image plusIcon;
    [SerializeField] private Image rightIcon;


    private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();


    public void Show(string s)
    {
        string[] buttonNames = s.Split('+');

        int n = buttonNames.Length;

        if (n == 1)
            ShowKeyboardButton(buttonNames[0]);
        else
            ShowKeyboardButtonCombination(buttonNames[0], buttonNames[1]);
    }

    public void ShowKeyboardButton(string buttonName)
    {
        if (buttonName == "Directionals")
        {
            directionalIcons.SetActive(true);
            leftIcon.gameObject.SetActive(false);
        }
        else
        {
            directionalIcons.SetActive(false);


            leftIcon.sprite = sprites["Keyboard_Black_" + buttonName + "_Up"];
            leftIcon.SetNativeSize();

            leftIcon.gameObject.SetActive(true);
        }
        
        
        plusIcon.gameObject.SetActive(false);
        rightIcon.gameObject.SetActive(false);
    }

    public void ShowKeyboardButtonCombination(string buttonName1, string buttonName2)
    {
        if (buttonName1 == "Directionals")
        {
            directionalIcons.SetActive(true);
            leftIcon.gameObject.SetActive(false);
        }
        else
        {
            directionalIcons.SetActive(false);


            leftIcon.sprite = sprites["Keyboard_Black_" + buttonName1 + "_Up"];
            leftIcon.SetNativeSize();

            leftIcon.gameObject.SetActive(true);
        }


        plusIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(16, 16);

        plusIcon.gameObject.SetActive(true);


        rightIcon.sprite = sprites["Keyboard_Black_" + buttonName2 + "_Up"];
        rightIcon.SetNativeSize();

        rightIcon.gameObject.SetActive(true);
    }

    public void Hide()
    {
        directionalIcons.SetActive(false);
        leftIcon.gameObject.SetActive(false);
        plusIcon.gameObject.SetActive(false);
        rightIcon.gameObject.SetActive(false);
    }


    private void Awake()
    {
        foreach (Sprite keyboardIcon in Resources.LoadAll<Sprite>(keyboardIcons))
            sprites.Add(keyboardIcon.name, keyboardIcon);

        foreach (Sprite joystickIcon in Resources.LoadAll<Sprite>(joystickIcons))
            sprites.Add(joystickIcon.name, joystickIcon);
    }
}
