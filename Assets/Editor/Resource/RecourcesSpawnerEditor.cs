using Examen.Spawning.ResourceSpawning;
using Examen.Spawning.ResourceSpawning.Structs;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Examen.Editors.Resource
{
    [CustomEditor(typeof(ResourceSpawner))]
    public class RecourcesSpawnerEditor : Editor
    {
        private static int _selectNumber;

        /// <summary>
        /// Draws Default inspector and adds a button, that spawns resources when clicked
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ResourceSpawner spawner = (ResourceSpawner)target;

            if (spawner.SpawnAreas.Count <= 0)
                return;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("All (Re)Spawn Resources"))
                spawner.SpawnAllResources();

            if (GUILayout.Button("All Remove Resources"))
                spawner.DestoryAllResources();

            EditorGUILayout.EndHorizontal();

            _selectNumber = EditorGUILayout.Popup("Select an option:", _selectNumber, SetToStringList(spawner.SpawnAreas));

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button($"(Re)Spawn resources in {spawner.SpawnAreas[_selectNumber].Area.gameObject.name}"))
                spawner.SpawnAreaResource(_selectNumber);

            if (GUILayout.Button($"Remove resources in {spawner.SpawnAreas[_selectNumber].Area.gameObject.name}"))
                spawner.DestroyAreaResources(_selectNumber);

            EditorGUILayout.EndHorizontal();

        }

        private string[] SetToStringList(List<ResourceSpawnAreas> spawnAreas)
        {
            string[] newStringArray = new string[spawnAreas.Count];

            for (int i = 0; i < spawnAreas.Count; i++)
                newStringArray[i] = spawnAreas[i].Area.gameObject.name;

            return newStringArray;
        }
    } 
}
