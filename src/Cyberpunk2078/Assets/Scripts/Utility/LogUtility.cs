/**
 * @author SerapH
 */

using UnityEngine;


public struct LogUtility
{
    public static string MakeLogString(string type, object content)
    {
        return "[" + System.DateTime.Now.ToShortTimeString() + "][" + type + "] " + content.ToString();
    }

    public static string MakeLogStringFormat(string type, string format, params object[] args)
    {
        return "[" + System.DateTime.Now.ToShortTimeString() + "][" + type + "] " + string.Format(format, args);
    }

    public static void PrintLog(string type, object content)
    {
#if UNITY_EDITOR
        Debug.Log(MakeLogString(type, content));
#endif
    }

    public static void PrintLogFormat(string type, string format, params object[] args)
    {
#if UNITY_EDITOR
        Debug.Log(MakeLogStringFormat(type, format, args));
#endif
    }

    public static void DrawGizmoArrow(Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
    {
#if UNITY_EDITOR
        Gizmos.DrawLine(from, to);

        Vector3 direction = (to - from).normalized;

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

        Gizmos.DrawRay(to, right * arrowHeadLength);
        Gizmos.DrawRay(to, left * arrowHeadLength);
#endif
    }
}
