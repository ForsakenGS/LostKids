using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(PuzzleSettings))]
public class RoomSettingsEditor : Editor {
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PuzzleSettings script = (PuzzleSettings)target;
        script.tags = (PuzzleTags2)EditorGUILayout.EnumMaskField("tags", script.tags);
    }
    
}
