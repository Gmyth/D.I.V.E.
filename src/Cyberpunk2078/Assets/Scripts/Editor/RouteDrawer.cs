using UnityEngine;
using UnityEditor;


[CustomPropertyDrawer(typeof(Route))]
public class RouteDrawer : PropertyDrawer
{
    private const float lineHeight = 16;
    private const float buttonWidth = 20;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty wayPointsProperty = property.FindPropertyRelative("wayPoints");
        

        EditorGUI.BeginProperty(position, label, property);

        int indent = EditorGUI.indentLevel;

        position.height = lineHeight;


        position = EditorGUI.PrefixLabel(position, new GUIContent(property.displayName));


        Rect contentPosition = position;


        contentPosition.width = 30f;

        EditorGUI.PropertyField(contentPosition, wayPointsProperty.FindPropertyRelative("Array.size"), GUIContent.none);

        if (wayPointsProperty.arraySize == 1)
            wayPointsProperty.ClearArray();
        else
        {
            contentPosition.x += contentPosition.width;


            contentPosition.width = buttonWidth;

            if (GUI.Button(contentPosition, "+"))
            {
                int numWayPoints = wayPointsProperty.arraySize;

                wayPointsProperty.InsertArrayElementAtIndex(numWayPoints);

                if (numWayPoints == 0)
                    wayPointsProperty.InsertArrayElementAtIndex(1);
            }

            contentPosition.x += contentPosition.width;

            if (GUI.Button(contentPosition, "-") && wayPointsProperty.arraySize > 0)
            {
                if (wayPointsProperty.arraySize == 2)
                    wayPointsProperty.ClearArray();
                else
                    wayPointsProperty.DeleteArrayElementAtIndex(wayPointsProperty.arraySize - 1);
            }
        }


        for (int i = 0; i < wayPointsProperty.arraySize; ++i)
        {
            position.y += position.height;

            EditorGUI.PropertyField(position, wayPointsProperty.GetArrayElementAtIndex(i), GUIContent.none);
        }


        SerializedProperty stayTimesProperty = property.FindPropertyRelative("stayTimes");


        position.y += position.height;


        EditorGUI.PropertyField(position, property.FindPropertyRelative("stayTimes"), true);


        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty wayPointsProperty = property.FindPropertyRelative("wayPoints");

        int numWayPoints = wayPointsProperty.arraySize;

        if (numWayPoints == 0)
            return lineHeight;


        SerializedProperty stayTimesProperty = property.FindPropertyRelative("stayTimes");


        return (1 + numWayPoints + (stayTimesProperty.isExpanded ? 2 + stayTimesProperty.arraySize : 1)) * lineHeight;
    }
}
