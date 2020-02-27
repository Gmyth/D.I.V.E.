using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Restorable: MonoBehaviour
{
    public bool saved = false;
    public bool Restored = false;

    public abstract void Restore();


    public abstract void Save();



}
