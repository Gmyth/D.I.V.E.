using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restorable : MonoBehaviour
{
    public bool saved = false;
    public bool Restored = false;

    public virtual void Restore()
    {
        Restored = true;
    }

    public virtual void Save()
    {
        saved = true;
    }


}
