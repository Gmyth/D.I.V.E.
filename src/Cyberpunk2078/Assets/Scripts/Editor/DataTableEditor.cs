using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;


[CustomEditor(typeof(DataTable), true, isFallback = true)]
public class DataTableEditor : Editor
{
    //public override void OnInspectorGUI()
    //{
    //    if (GUILayout.Button("EDIT"))
    //    {
    //        DataTableEditorWindow window = (DataTableEditorWindow)EditorWindow.GetWindow(typeof(DataTableEditorWindow));

    //        window.serializedEntries = serializedObject.FindProperty("serializedEntries");
    //        window.titleContent = new GUIContent(serializedObject.targetObject.name);
    //    }
    //}
}


public class DataTableEditorWindow : EditorWindow
{
    private const float buttonWidth = 20;


    [MenuItem("Window/Data Tables")]
    static void ShowWindow()
    {
        DataTableEditorWindow window = GetWindow<DataTableEditorWindow>();


        window.titleContent = new GUIContent("Data Tables");
        window.Show();
    }


    private TreeViewState tableState;
    private TableView table;

    public SerializedProperty serializedEntries;


    private void OnEnable()
    {
    }

    private void OnGUI()
    {
        if (serializedEntries == null)
        {
            titleContent = new GUIContent("Data Tables");
            table = null;
        }
        else
        {
            Rect contentPosition = new Rect(new Vector2(5, 5), new Vector2(position.width - 5, 25));


            int N = serializedEntries.arraySize;

            contentPosition.width = buttonWidth;
            if (GUI.Button(contentPosition, "+"))
            {
                serializedEntries.InsertArrayElementAtIndex(N);
            }

            contentPosition.x += buttonWidth;
            if (GUI.Button(contentPosition, "-") && N > 0)
                serializedEntries.DeleteArrayElementAtIndex(N - 1);


            contentPosition.x = 5;
            contentPosition.y += contentPosition.height + 5;


            if (serializedEntries.arraySize == 0)
                table = null;
            else
            {
                if (table == null)
                {
                    //SerializedProperty property = serializedEntries.GetArrayElementAtIndex(0);

                    //List<MultiColumnHeaderState.Column> list = new List<MultiColumnHeaderState.Column>();

                    //property.NextVisible(true);

                    //list.Add(new MultiColumnHeaderState.Column() { allowToggleVisibility = true, autoResize = true, canSort = true, headerContent = new GUIContent(property.name), minWidth = 20, maxWidth = 100 });


                    //while (property.NextVisible(false))
                    //    list.Add(new MultiColumnHeaderState.Column() { allowToggleVisibility = true, autoResize = true, canSort = true, headerContent = new GUIContent(property.name), minWidth = 20, maxWidth = 100 });

                    //contentPosition.width = 1000;

                    if (tableState == null)
                        tableState = new TreeViewState();


                    table = new TableView(tableState, serializedEntries);
                }


                contentPosition.height = position.height - contentPosition.height - 15;

                table.OnGUI(contentPosition);
            }
        }
    }
}
