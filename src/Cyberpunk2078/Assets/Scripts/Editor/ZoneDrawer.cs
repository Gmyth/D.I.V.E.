using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(Zone), true)]
public class ZoneDrawer : PropertyDrawer
{
    private const float heightPerLine = 16;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty typeProperty = property.FindPropertyRelative("type");
        SerializedProperty centerProperty = property.FindPropertyRelative("center");
        SerializedProperty widthProperty = property.FindPropertyRelative("w");
        SerializedProperty heightProperty = property.FindPropertyRelative("h");


        EditorGUI.BeginProperty(position, label, property);

        int indent = EditorGUI.indentLevel;

        position.height = heightPerLine;


        EditorGUI.PropertyField(EditorGUI.PrefixLabel(position, new GUIContent(property.displayName)), typeProperty, GUIContent.none);

        ++EditorGUI.indentLevel;

        position.y += position.height;


        EditorGUI.PropertyField(position, centerProperty, new GUIContent("Center"));


        position.y += position.height;


        switch (typeProperty.enumValueIndex)
        {
            case (int)ZoneType.Rectangle:
                EditorGUI.PropertyField(position, widthProperty, new GUIContent("Width"));

                position.y += position.height;

                EditorGUI.PropertyField(position, heightProperty, new GUIContent("Height"));

                Vector3[] v = { new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 0) };
                Handles.DrawSolidRectangleWithOutline(v, Color.white, Color.black);

                break;


            case (int)ZoneType.Circle:
                EditorGUI.PropertyField(position, widthProperty, new GUIContent("Radius"));
                break;
        }


        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty typeProperty = property.FindPropertyRelative("type");

        switch (typeProperty.enumValueIndex)
        {
            case (int)ZoneType.Circle:
                return 3 * heightPerLine;


            default:
                return 4 * heightPerLine;
        }
    }
}
