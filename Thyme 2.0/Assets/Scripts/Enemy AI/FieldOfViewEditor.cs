using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Chase))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        Chase fow = (Chase)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360f, fow.viewRadius);
        Vector3 vieuwAngleA = fow.DirFromAngle(-fow.viewAngle * 0.5f, false);
        Vector3 vieuwAngleB = fow.DirFromAngle(fow.viewAngle * 0.5f, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + vieuwAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + vieuwAngleB * fow.viewRadius);
    }
}
