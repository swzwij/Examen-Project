using Examen.Spawning.ResourceSpawning;
using UnityEditor;
using UnityEngine;

namespace Examen.Editors.Resource
{
    [CustomEditor(typeof(ResourceSpawner))]
    public class RecourcesSpawnerEditor : Editor
    {
        /// <summary>
        /// Draws Default inspector and adds a button, that spawns resources when clicked
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ResourceSpawner spawner = (ResourceSpawner)target;

            if (GUILayout.Button("Spawn Resources"))
                spawner.InitializedSpawning();
        }
    }
}