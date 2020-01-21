using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[CustomEditor(typeof(CameraIndicator), true)]
public class CameraEditor : Editor
{

    private static readonly Vector3[] handlePoints = new[] { new Vector3(0.0f, 0, 0.5f), new Vector3(1.0f, 0, 0.5f), new Vector3(0.5f, 0, 0.0f), new Vector3(0.5f, 0, 1.0f) };

    private CameraIndicator indicator = null;
    private Vector2 size;

    public override void OnInspectorGUI()
    {
        CameraIndicator new_indicator = target as CameraIndicator;
        
        if (new_indicator != indicator)
        {
            indicator = new_indicator;

            Vector3 cameraInitPosition = indicator.transform.position;
            cameraInitPosition.z = -10;
            
           

//            if (indicator.maxCameraPos.x < 9999)
//            {
//                cameraInitPosition.x = indicator.maxCameraPos.x;
//                if (indicator.maxCameraPos.y < 9999)
//                    cameraInitPosition.y = indicator.maxCameraPos.y;
//                else
//                    cameraInitPosition.y = indicator.minCameraPos.y;
//            }
//            else
//            {
//                cameraInitPosition.x = indicator.minCameraPos.x;
//                if (indicator.maxCameraPos.y < 9999)
//                    cameraInitPosition.y = indicator.maxCameraPos.y;
//                else
//                    cameraInitPosition.y = indicator.minCameraPos.y;
//            }
//            
//            Camera.main.transform.position = cameraInitPosition;
//
//            if (indicator.changeSize)
//            {
//                Camera.main.orthographicSize = indicator.targetCameraSize;
//            }
//            else
//            {
//                Camera.main.orthographicSize = 8;
//            }
        }

        DrawDefaultInspector();
    }
    private Vector2 getCameraSize(float customSize = -1)
    {
        Camera cam = Camera.main;
        float height = customSize==-1 ? 2f * cam.orthographicSize : 2f * customSize;
        float width = height * cam.aspect;
        return new Vector2(width,height);
    }

    private void OnSceneGUI()
    {
        if (!indicator)
            return;

        if (Tools.current == Tool.Custom && EditorTools.activeToolType == typeof(CameraTool))
        {
            Vector2 center = indicator.transform.position;
            float width = indicator.width;
            float height = indicator.length;

            Vector3 cameraInitPosition = indicator.transform.position;
            cameraInitPosition.z = -10;
            
            if (indicator.changeSize)
            {
                size = getCameraSize(indicator.targetCameraSize);
            }
            else
            {
                size = getCameraSize(8);
            }
    

            if (width * height != 0)
            {
                EditorGUI.BeginChangeCheck();

                Vector3 V = Handles.PositionHandle(center, Quaternion.identity);
                Handles.Label(V, "Camera Area");

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(indicator, "Move Camera Area");
                    indicator.transform.position = V;
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    
                    Vector2 minPos = indicator.minCameraPos;
                    Vector2 maxPos = indicator.maxCameraPos;
                    
                    Vector2 size = new Vector2(width, height);
                    Handles.DrawSolidRectangleWithOutline(new Rect(center - size / 2f, size), new Color(1f, 0f, 0f, 0.2f), new Color(1f, 0f, 0f, 0.2f));

                    Handles.CapFunction cap = Handles.CubeHandleCap;
                    //CameraBound
                    if (indicator.bounds)
                    {
                        Handles.color = Color.yellow;
                        minPos = Handles.PositionHandle(minPos, Quaternion.identity);
                        Handles.Label(minPos, "Camera Min Bound");
                        maxPos = Handles.PositionHandle(maxPos, Quaternion.identity);
                        Handles.Label(maxPos, "Camera Max Bound");
                    }

                    // Camera Size
                    Vector3 c = center;
                    Vector2 w = new Vector2(width / 2, 0);
                    Vector2 h = new Vector2(0, height / 2);

                    Vector3 rightHandlePosition = center + w;
                    Vector3 leftHandlePosition = center - w;
                    Vector3 topHandlePosition = center + h;
                    Vector3 bottomHandlePosition = center - h;

                    Handles.color = Color.red;
                    rightHandlePosition = Handles.Slider(rightHandlePosition, rightHandlePosition - c, 0.2f, cap, 0);
                    leftHandlePosition = Handles.Slider(leftHandlePosition, leftHandlePosition - c, 0.2f, cap, 0);

                    Handles.color = Color.green;
                    topHandlePosition = Handles.Slider(topHandlePosition, topHandlePosition - c, 0.2f, cap, 0);
                    bottomHandlePosition = Handles.Slider(bottomHandlePosition, bottomHandlePosition - c, 0.2f, cap, 0);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(indicator, "Adjust Camera Area");

                        indicator.transform.position = new Vector2((rightHandlePosition.x + leftHandlePosition.x) / 2f, (topHandlePosition.y + bottomHandlePosition.y) / 2f);
                        indicator.width = rightHandlePosition.x - leftHandlePosition.x;
                        indicator.length = topHandlePosition.y - bottomHandlePosition.y;
                        if (indicator.bounds)
                        {
                            indicator.minCameraPos = minPos; 
                            indicator.maxCameraPos = maxPos;
                        }
                    }
                }
            }
            
            Handles.Label(indicator.maxCameraPos - size/2, "maxPos");
            Handles.Label(indicator.minCameraPos - size/2, "minPos");
            Handles.DrawSolidRectangleWithOutline(new Rect(indicator.maxCameraPos.x - size.x / 2, indicator.maxCameraPos.y - size.y / 2,size.x, size.y), new Color(0.0f, 0f, 0f, 0f), new Color(1, 0.6f, 1, 1));
            Handles.DrawSolidRectangleWithOutline(new Rect(indicator.minCameraPos.x - size.x / 2, indicator.minCameraPos.y - size.y / 2,size.x, size.y), new Color(0.0f, 0f, 0f, 0f), new Color(0.6f, 1, 1, 1));

        }
    }
}
