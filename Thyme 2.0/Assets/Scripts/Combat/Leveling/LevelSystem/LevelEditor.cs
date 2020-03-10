using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    private SerializedProperty stats;

    private void OnEnable()
    {
        stats = serializedObject.FindProperty("statIncrease");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(serializedObject.FindProperty("expNeeded"), new GUIContent("Exp needed:"), true);
        
        stats.arraySize = EditorGUILayout.IntField("Size", stats.arraySize);
        for (int i = 0; i < stats.arraySize; i++)
        {
            var stat = stats.GetArrayElementAtIndex(i);
            EditorGUILayout.PropertyField(stat, new GUIContent("Stat To Be Adjusted " + i), true);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
