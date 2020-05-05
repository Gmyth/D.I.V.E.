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
    public static GUITutorial Singleton { get; private set; }


    [SerializeField] private string keyboardIcons;
    [SerializeField] private string joystickIcons;
    [SerializeField] private Sprite mouseLeftIcon;
    [SerializeField] private Sprite mouseRightIcon;
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
            Show(buttonNames[0], "");
        else
            Show(buttonNames[0], buttonNames[1]);
    }

    public void Show(string buttonName1, string buttonName2)
    {
        //Debug.Log(buttonName1);
        //Debug.Log(buttonName2);
        InputType currentInputType = MouseIndicator.Singleton.CurrentInputType;


        if (buttonName1 == "Directionals")
        {
            directionalIcons.SetActive(true);
            leftIcon.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning(buttonName1);
            leftIcon.sprite = sprites[keyNames[buttonName1][(int)currentInputType]];
            leftIcon.SetNativeSize();

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
            rightIcon.SetNativeSize();

            plusIcon.gameObject.SetActive(true);
            rightIcon.gameObject.SetActive(true);
        }
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
        if (Singleton)
            Destroy(gameObject);
        else
        {
            Singleton = this;


            foreach (Sprite keyboardIcon in Resources.LoadAll<Sprite>(keyboardIcons))
                sprites.Add(keyboardIcon.name, keyboardIcon);

            foreach (Sprite joystickIcon in Resources.LoadAll<Sprite>(joystickIcons))
                sprites.Add(joystickIcon.name, joystickIcon);

            sprites.Add("Keyboard_Black_MouseLeft_Up", mouseLeftIcon);
            sprites.Add("Keyboard_Black_MouseRight_Up", mouseRightIcon);


            foreach (KeyBindingData data in bindingData)
                keyNames.Add(data.ButtonName, new string[2] { string.Format("Keyboard_Black_{0}_Up", data.KeyboardButtonName), string.Format("Xbox_One_Large_{0}_Down", data.JoystickButtonName) });
        }
    }
}
