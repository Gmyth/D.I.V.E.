using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Drone))]
public class DroneEditor : Editor
{
    bool IsFoldout = false;
    public override void OnInspectorGUI ()
    {
        DrawDefaultInspector();
        Drone Target = (Drone)target;
 
        IsFoldout = EditorGUILayout.Foldout(IsFoldout, "Guard Zone");
         
        if (IsFoldout)
        {
            if (Selection.activeTransform)
            {
                Selection.activeTransform.position =
                    EditorGUILayout.Vector3Field("Position", Selection.activeTransform.position);
                // status = Selection.activeTransform.name;
            }
        }
         
        if (GUILayout.Button("Set Center"))
        {
            Target.setCenter();
        }
         
         
        IsFoldout = EditorGUILayout.Foldout(IsFoldout, "Patrol Points");

        if (IsFoldout)
        {
            if (Selection.activeTransform)
            {
                Selection.activeTransform.position =
                    EditorGUILayout.Vector3Field("Position", Selection.activeTransform.position);
                // status = Selection.activeTransform.name;
            }
        }
             
 
         
        for(int i = 0; i < Target.patrolPoints.Length; ++i)
        {
            if (GUILayout.Button("Set Point For " + i))
            {
                Target.patrolPoints[i] = Target.Pos;
            }
        }
         

    }
     
    public void OnInspectorUpdate()
    {
        this.Repaint();
    }
}