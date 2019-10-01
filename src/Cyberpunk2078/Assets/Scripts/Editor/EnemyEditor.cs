using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;


[CustomEditor(typeof(Enemy), true)]
public class EnemyEditor : Editor
{
    private static readonly Vector3[] handlePoints = new[] { new Vector3(0.0f, 0, 0.5f), new Vector3(1.0f, 0, 0.5f), new Vector3(0.5f, 0, 0.0f), new Vector3(0.5f, 0, 1.0f) };


    private Enemy enemy = null;


    public override void OnInspectorGUI()
    {
        enemy = target as Enemy;

        DrawDefaultInspector();
    }


    private void OnSceneGUI()
    {
        if (!enemy)
            return;


        Event e = Event.current;

        
        if (Tools.current == Tool.Custom && EditorTools.activeToolType == typeof(EnemyTool))
        {
            Zone guardZone = enemy.GuardZone;

            if (guardZone.Width * guardZone.Height != 0)
                switch (guardZone.Type)
                {
                    case ZoneType.Rectangle:
                        {
                            Vector2 size = new Vector2(guardZone.Width, guardZone.Height);
                            Handles.DrawSolidRectangleWithOutline(new Rect(guardZone.center - size / 2f, size), new Color(1f, 0f, 0f, 0.2f), new Color(1f, 0f, 0f, 0.2f));

                            Handles.CapFunction cap = Handles.CubeHandleCap;

                            Vector3 c = guardZone.center;
                            Vector2 w = new Vector2(guardZone.Width / 2, 0);
                            Vector2 h = new Vector2(0, guardZone.Height / 2);

                            EditorGUI.BeginChangeCheck();

                            Vector3 rightHandlePosition = guardZone.center + w;
                            Vector3 leftHandlePosition = guardZone.center - w;
                            Vector3 topHandlePosition = guardZone.center + h;
                            Vector3 bottomHandlePosition = guardZone.center - h;

                            Handles.color = Color.red;
                            rightHandlePosition = Handles.Slider(rightHandlePosition, rightHandlePosition - c, 0.2f, cap, 0);
                            leftHandlePosition = Handles.Slider(leftHandlePosition, leftHandlePosition - c, 0.2f, cap, 0);

                            Handles.color = Color.green;
                            topHandlePosition = Handles.Slider(topHandlePosition, topHandlePosition - c, 0.2f, cap, 0);
                            bottomHandlePosition = Handles.Slider(bottomHandlePosition, bottomHandlePosition - c, 0.2f, cap, 0);

                            if (EditorGUI.EndChangeCheck())
                            {
                                guardZone.center.x = (rightHandlePosition.x + leftHandlePosition.x) / 2f;
                                guardZone.center.y = (topHandlePosition.y + bottomHandlePosition.y) / 2f;
                                guardZone.Width = rightHandlePosition.x - leftHandlePosition.x;
                                guardZone.Height = topHandlePosition.y - bottomHandlePosition.y;
                            }
                        }
                        break;


                    case ZoneType.Circle:
                        {
                            Handles.color = new Color(1f, 0f, 0f, 0.2f);
                            Handles.DrawSolidDisc(guardZone.center, Vector3.forward, guardZone.Radius);

                            Handles.CapFunction cap = Handles.CubeHandleCap;

                            Vector3 c = guardZone.center;
                            Vector2 w = new Vector2(guardZone.Radius, 0);
                            Vector2 h = new Vector2(0, guardZone.Radius);

                            Vector3 rightHandlePosition = guardZone.center + w;
                            Vector3 leftHandlePosition = guardZone.center - w;
                            Vector3 topHandlePosition = guardZone.center + h;
                            Vector3 bottomHandlePosition = guardZone.center - h;

                            Handles.color = Color.red;

                            EditorGUI.BeginChangeCheck();
                            rightHandlePosition = Handles.Slider(rightHandlePosition, rightHandlePosition - c, 0.2f, cap, 0);
                            leftHandlePosition = Handles.Slider(leftHandlePosition, leftHandlePosition - c, 0.2f, cap, 0);
                            if (EditorGUI.EndChangeCheck())
                            {
                                guardZone.center.x = (rightHandlePosition.x + leftHandlePosition.x) / 2f;
                                guardZone.Radius = (rightHandlePosition.x - leftHandlePosition.x) / 2f;
                                break;
                            }

                            Handles.color = Color.green;

                            EditorGUI.BeginChangeCheck();
                            topHandlePosition = Handles.Slider(topHandlePosition, topHandlePosition - c, 0.2f, cap, 0);
                            bottomHandlePosition = Handles.Slider(bottomHandlePosition, bottomHandlePosition - c, 0.2f, cap, 0);
                            if (EditorGUI.EndChangeCheck())
                            {
                                guardZone.center.y = (topHandlePosition.y + bottomHandlePosition.y) / 2f;
                                guardZone.Radius = (topHandlePosition.y - bottomHandlePosition.y) / 2f;
                                break;
                            }
                        }
                        break;
                }


            foreach (FieldInfo fieldInfo in enemy.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                try
                {
                    if (fieldInfo.GetCustomAttribute<PathAttribute>(false) != null)
                    {
                        Type fieldType = fieldInfo.FieldType;

                        if (fieldType == typeof(Route))
                        {
                            Route route = fieldInfo.GetValue(enemy) as Route;
                            Type t = route.GetType();
                            Vector3[] wayPoints = t.GetField("wayPoints", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(route) as Vector3[];

                            if (wayPoints.Length > 0)
                            {
                                for (int i = 0; i < wayPoints.Length; ++i)
                                {
                                    EditorGUI.BeginChangeCheck();

                                    Handles.color = new Color(0f, 0f, 1f, 0.8f);
                                    Handles.DrawSolidDisc(wayPoints[i], Vector3.forward, 0.1f);
                                    Vector3 V = Handles.PositionHandle(wayPoints[i], Quaternion.identity);
                                    Handles.Label(V, i.ToString());

                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        wayPoints[i] = V;
                                        break;
                                    }
                                }


                                for (int i = 0; i < wayPoints.Length - 1; ++i)
                                    LogUtility.DrawGizmoArrow(wayPoints[i], wayPoints[i + 1]);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
