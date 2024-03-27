using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResourceSpawner))]
public class RecourcesSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ResourceSpawner spawner = (ResourceSpawner)target;

        if (GUILayout.Button("Spawn Resources"))
            spawner.InitializedSpawning();

   
    }
}
