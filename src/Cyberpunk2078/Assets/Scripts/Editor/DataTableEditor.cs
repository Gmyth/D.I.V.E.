using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;


[CustomEditor(typeof(DataTable), true, isFallback = true)]
public class DataTableEditor : Editor
{
    private DataTableEditorWindow editorWindow;


    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("EDIT"))
            ((DataTableEditorWindow)EditorWindow.GetWindow(typeof(DataTableEditorWindow))).serializedEntries = serializedObject.FindProperty("serializedEntries");
    }
}


public class DataTableEditorWindow : EditorWindow
{
    private const float buttonWidth = 20;


    public SerializedProperty serializedEntries;

    private TreeView treeView;
    private TreeViewState treeViewState;

    private MultiColumnHeader header;
    private MultiColumnHeaderState headerState;


    void OnGUI()
    {
        if (serializedEntries == null)
        {
            header = null;
        }
        else
        {
            Rect contentPosition = new Rect(new Vector2(5, 5), new Vector2(position.width - 5, 25));


            int N = serializedEntries.arraySize;

            contentPosition.width = buttonWidth;
            if (GUI.Button(contentPosition, "+"))
                serializedEntries.InsertArrayElementAtIndex(N);

            contentPosition.x += buttonWidth;
            if (GUI.Button(contentPosition, "-") && N > 0)
                serializedEntries.DeleteArrayElementAtIndex(N - 1);


            contentPosition.x = 5;
            contentPosition.y += contentPosition.height + 5;


            if (serializedEntries.arraySize == 0)
                header = null;
            else
            {
                if (header == null)
                {
                    SerializedProperty property = serializedEntries.GetArrayElementAtIndex(0);

                    List<MultiColumnHeaderState.Column> list = new List<MultiColumnHeaderState.Column>();

                    property.NextVisible(true);

                    list.Add(new MultiColumnHeaderState.Column() { allowToggleVisibility = true, autoResize = true, canSort = true, headerContent = new GUIContent(property.name), minWidth = 20, maxWidth = 100 });


                    while (property.NextVisible(false))
                        list.Add(new MultiColumnHeaderState.Column() { allowToggleVisibility = true, autoResize = true, canSort = true, headerContent = new GUIContent(property.name), minWidth = 20, maxWidth = 100 });

                    contentPosition.width = 1000;


                    header = new MultiColumnHeader(new MultiColumnHeaderState(list.ToArray()));

                    header.ResizeToFit();
                }
                

                header.OnGUI(contentPosition, 0);
            }
        }
    }
}
