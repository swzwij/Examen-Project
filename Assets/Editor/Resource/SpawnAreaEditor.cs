using Examen.Spawning.ResourceSpawning;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnArea))]
public class SpawnAreaEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        DrawDefaultInspector();
        
        if(GUILayout.Button("Update Spawn Area"))
            ((SpawnArea)target).UpdateArea();
    }
}
