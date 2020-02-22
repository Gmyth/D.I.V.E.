using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class KeyBindingData
{
    [SerializeField] private string buttonName;
    [SerializeField] private string keyboardButtonName;
    [SerializeField] private string joystickButtonName;


    public string ButtonName
    {
        get
        {
            return buttonName;
        }
    }

    public string KeyboardButtonName
    {
        get
        {
            return keyboardButtonName;
        }
    }

    public string JoystickButtonName
    {
        get
        {
            return joystickButtonName;
        }
    }
}


public class GUITutorial : MonoBehaviour
{
    [SerializeField] private string keyboardIcons;
    [SerializeField] private string joystickIcons;
    [SerializeField] private GameObject directionalIcons;
    [SerializeField] private Image leftIcon;
    [SerializeField] private Image plusIcon;
    [SerializeField] private Image rightIcon;

    [Header("Key Binding")]
    [SerializeField] private KeyBindingData[] bindingData;

    private Dictionary<string, string[]> keyNames = new Dictionary<string, string[]>();
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

    public void Show(string buttonName1, string buttonName2 = "")
    {
        InputType currentInputType = MouseIndicator.Singleton.CurrentInputType;


        if (buttonName1 == "Directionals")
        {
            directionalIcons.SetActive(true);
            leftIcon.gameObject.SetActive(false);
        }
        else
        {
            leftIcon.sprite = sprites[keyNames[buttonName1][(int)currentInputType]];

            directionalIcons.SetActive(false);
            leftIcon.gameObject.SetActive(true);
        }


        if (buttonName2 == "")
        {
            plusIcon.gameObject.SetActive(false);
            rightIcon.gameObject.SetActive(false);
        }
        else
        {
            rightIcon.sprite = sprites[keyNames[buttonName2][(int)currentInputType]];

            plusIcon.gameObject.SetActive(true);
            rightIcon.gameObject.SetActive(true);
        }
    }

    public void ShowKeyboardButton(string buttonName)
    {
        if (buttonName == "Keyboard_WASD")
        {
            directionalIcons.SetActive(true);
            leftIcon.gameObject.SetActive(false);
        }
        else
        {
            directionalIcons.SetActive(false);


            leftIcon.sprite = sprites[buttonName];
            leftIcon.SetNativeSize();

            leftIcon.gameObject.SetActive(true);
        }
        
        
        plusIcon.gameObject.SetActive(false);
        rightIcon.gameObject.SetActive(false);
    }

    public void ShowKeyboardButtonCombination(string buttonName1, string buttonName2)
    {
        if (buttonName1 == "Keyboard_WASD")
        {
            directionalIcons.SetActive(true);
            leftIcon.gameObject.SetActive(false);
        }
        else
        {
            directionalIcons.SetActive(false);


            leftIcon.sprite = sprites[buttonName1];
            leftIcon.SetNativeSize();

            leftIcon.gameObject.SetActive(true);
        }


        plusIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(16, 16);

        plusIcon.gameObject.SetActive(true);


        rightIcon.sprite = sprites[buttonName2];
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

        foreach (KeyBindingData data in bindingData)
            keyNames.Add(data.ButtonName, new string[2] { string.Format("Keyboard_Black_{0}_Up", data.KeyboardButtonName), string.Format("Xbox_One_Large_{0}_Up", data.JoystickButtonName) });
    }
}
