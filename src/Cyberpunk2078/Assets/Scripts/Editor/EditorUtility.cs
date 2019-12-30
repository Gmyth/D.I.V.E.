using System;
using FMODUnity;
using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
public class EditorUtility
{
    public static void DrawHandlesArrow(Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {

        if (Vector3.Distance(from, to) == 0)
            return;


        Handles.DrawLine(from, to);

        Vector3 direction = (to - from).normalized;

        Vector3 right = Quaternion.Euler(0, 0, 180 + arrowHeadAngle) * direction;
        Vector3 left = Quaternion.Euler(0, 0, 180 - arrowHeadAngle) * direction;

        Handles.DrawLine(to, to + right * arrowHeadLength);
        Handles.DrawLine(to, to + left * arrowHeadLength);
    }

    internal static void SetDirty(Settings settings)
    {
        throw new NotImplementedException();
    }

    internal static bool DisplayDialog(string v1, string v2, string v3, string v4)
    {
        throw new NotImplementedException();
    }
}
#endif
