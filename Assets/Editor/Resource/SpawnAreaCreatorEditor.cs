using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnAreaCreator))]
public class SpawnAreaCreatorEditor : Editor 
{
    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
        
        if(GUILayout.Button("Create Spawn Area"))
            ((SpawnAreaCreator)target).CreateSpawnArea();

        if(GUILayout.Button("Toggle Lines"))
            ((SpawnAreaCreator)target).ToggleLines();
    }
}

