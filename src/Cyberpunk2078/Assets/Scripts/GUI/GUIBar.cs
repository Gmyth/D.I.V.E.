using UnityEngine;
using UnityEngine.UI;


public class GUIBar : GUIWidget
{
    [SerializeField] private Image[] foregrounds;


    public float Value
    {
        get
        {
            return foregrounds[0].fillAmount;
        }

        set
        {
            foreach (Image image in foregrounds)
                image.fillAmount = value;
        }
    }

    public Color Color
    {
        set
        {
            foreach (Image image in foregrounds)
                image.color = value;
        }
    }
}
