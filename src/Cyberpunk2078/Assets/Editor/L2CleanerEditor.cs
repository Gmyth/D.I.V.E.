
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(L2Cleaner))]
public class L2CleanerEditor : Editor
{
    bool IsFoldout = false;
    public override void OnInspectorGUI ()
    {
        DrawDefaultInspector();
        L2Cleaner Target = (L2Cleaner)target;
 
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
            //Target.setCenter();
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
    }
     

    public void OnInspectorUpdate()
    {
        Repaint();
    }
}