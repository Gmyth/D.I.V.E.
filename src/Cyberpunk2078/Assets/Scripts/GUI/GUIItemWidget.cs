using UnityEngine;
using UnityEngine.UI;


public class GUIItemWidget : GUIWidget
{
    public override void Refresh(params object[] args)
    {
        uint n = (uint)args[0];


        for (int i = 0; i < transform.childCount; ++i)
            transform.GetChild(i).GetComponent<Image>().color = i < n ? Color.white : Color.grey;
    }
}
