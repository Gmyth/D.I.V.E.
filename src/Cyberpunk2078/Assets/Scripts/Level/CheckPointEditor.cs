//using System.Collections;
//using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CheckPointManager))]
public class CheckPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CheckPointManager myScript = (CheckPointManager)target;
        if (GUILayout.Button("Set Check Point"))
        {
            myScript.SetCheckPoint();
        }

        if (GUILayout.Button("Clear Check Point"))
        {
            myScript.ClearCheckPoint();
        }
    }
}
#endif