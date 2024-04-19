using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnAreaCreator))]
public class SpawnAreaCreatorEditor : Editor 
{
    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
        
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Create Spawn Area"))
            ((SpawnAreaCreator)target).CreateSpawnArea();

        if(GUILayout.Button("Clear Spawn Areas"))
            ((SpawnAreaCreator)target).ClearSpawnAreas();
        
        EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Toggle Lines"))
            ((SpawnAreaCreator)target).ToggleLines();
    }
}

