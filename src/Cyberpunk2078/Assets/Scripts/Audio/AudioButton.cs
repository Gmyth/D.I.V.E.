using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class AudioButton : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    // When highlighted with mouse.
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Do something.
        AudioManager.Singleton.PlayOnce("Menu_move");
    }
    // When selected.
    public void OnSelect(BaseEventData eventData)
    {
                // Do something.
        AudioManager.Singleton.PlayOnce("Menu_select");
    }

    
}