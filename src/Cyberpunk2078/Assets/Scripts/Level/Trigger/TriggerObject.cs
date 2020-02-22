using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract  class TriggerObject : MonoBehaviour
{
    // Start is called before the first frame update

    public abstract void Enable();
    
    public abstract void Disable();
}
