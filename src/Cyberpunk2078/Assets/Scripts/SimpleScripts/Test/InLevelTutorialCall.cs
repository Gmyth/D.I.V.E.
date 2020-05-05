using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InLevelTutorialCall : MonoBehaviour
{

    public void Call_NormalEnergyIntroduce(TimelineManager timelineManager)
    {
        SimpleTutorialManager.Instance.IntroduceNormalEnergy(timelineManager);
    }

    public void Call_BulletDeflectIntroduce(TimelineManager timelineManager)
    {
        SimpleTutorialManager.Instance.IntroduceBulletDeflect(timelineManager);
    }

    public void Call_PowerDashIntroduce(TimelineManager timelineManager)
    {
        PowerDashTutorial.Instance.IntroducePowerDash(timelineManager);
    }

}
