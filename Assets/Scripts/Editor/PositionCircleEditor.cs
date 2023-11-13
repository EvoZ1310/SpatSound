using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PositionCircle))]
public class PositionSCircleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Reload circle"))
        {
            (target as PositionCircle).ReloadCircle();
        }
    }
}

