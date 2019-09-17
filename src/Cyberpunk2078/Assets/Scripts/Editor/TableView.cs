using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;


public class TableView : TreeView
{
    private SerializedProperty serializedEntries;

    private TreeViewItem root;
    private List<TreeViewItem> rows;


    public TableView(TreeViewState state, SerializedProperty serializedEntries) : base(state)
    {
        this.serializedEntries = serializedEntries;

        Reload();
    }


    public override void OnGUI(Rect rect)
    {
        Reload();

        base.OnGUI(rect);
    }

    protected override TreeViewItem BuildRoot()
    {
        root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };


        SetupDepthsFromParentsAndChildren(root);


        return root;
    }

    protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
    {
        int N = serializedEntries.arraySize;


        for (int i = 0; i < N; ++i)
        {
            SerializedProperty item = serializedEntries.GetArrayElementAtIndex(i);

            root.AddChild(new TreeViewItem { id = 0, displayName = item.displayName });
        }


        return base.BuildRows(root);
    }
}
