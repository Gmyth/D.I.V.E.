using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraIndicatorSetter : EditorWindow
{
    private static Dictionary<CameraIndicator, Vector2> lastPositions = new Dictionary<CameraIndicator, Vector2>();
    
    
    [MenuItem("Tools/Auto Set CameraIndicators in Children")]
    private static void AutoSetCameraIndicators()
    {
        Transform parent = Selection.activeGameObject.transform;
        CameraIndicator[] cameraIndicators = parent.GetComponentsInChildren<CameraIndicator>();
        
        GameObject[] gameObjects = new GameObject[cameraIndicators.Length];
        for (int i = 0; i < cameraIndicators.Length; ++i)
            gameObjects[i] = cameraIndicators[i].gameObject;

        Undo.RecordObjects(gameObjects, "Auto Set Camera Indicator");

        
        Vector2 position = (Vector2) parent.position;
        
        foreach (CameraIndicator cameraIndicator in cameraIndicators)
            if (!lastPositions.ContainsKey(cameraIndicator) )
            {
                cameraIndicator.minCameraPos = cameraIndicator.minCameraPos + position;
                cameraIndicator.maxCameraPos = cameraIndicator.maxCameraPos + position;
                lastPositions[cameraIndicator] = position;
            }else if (lastPositions[cameraIndicator] != position)
            {
                cameraIndicator.minCameraPos = cameraIndicator.minCameraPos - lastPositions[cameraIndicator] + position;
                cameraIndicator.maxCameraPos = cameraIndicator.maxCameraPos - lastPositions[cameraIndicator] + position;
                lastPositions[cameraIndicator] = position;
            }
    }
}
