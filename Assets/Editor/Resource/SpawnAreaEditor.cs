using Examen.Spawning.ResourceSpawning;
using UnityEditor;
using UnityEngine;

namespace Examen.Editors.Resource
{
    [CustomEditor(typeof(SpawnArea))]
    public class SpawnAreaEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Update Spawn Area"))
                ((SpawnArea)target).UpdateArea();
        }
    }
}