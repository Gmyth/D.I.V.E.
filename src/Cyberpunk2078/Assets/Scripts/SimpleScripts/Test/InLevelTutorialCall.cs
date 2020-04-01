using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InLevelTutorialCall : MonoBehaviour
{

    public void Call_IntroducePowerDash(TimelineManager timelineManager)
    {
        SimpleTutorialManager.Instance.IntroducePowerDash(timelineManager);
    }

}
