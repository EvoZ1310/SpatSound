using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MicInput))]
public class MicInputEditor : Editor
{
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Start listening"))
        {
            (target as MicInput).StartListening();
        }
        if (GUILayout.Button("Stop listening"))
        {
            (target as MicInput).StopListening();
        }
    }
}
