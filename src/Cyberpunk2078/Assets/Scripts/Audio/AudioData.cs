using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio/AudioData", fileName = "Audio Data")]
public class AudioData : ScriptableObject
{
    [FMODUnity.EventRef]
    public List<string> events;

}
