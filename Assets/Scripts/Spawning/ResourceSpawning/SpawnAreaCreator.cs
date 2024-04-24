using System.Collections.Generic;
using Examen.Spawning.ResourceSpawning.Structs;
using Unity.VisualScripting;
using UnityEngine;

namespace Examen.Spawning.ResourceSpawning
{
    [RequireComponent(typeof(LineRenderer), typeof(ResourceSpawner))]
    public class SpawnAreaCreator : MonoBehaviour
    {
        [SerializeField] private ResourceSpawner _resourceSpawner;
        [SerializeField] private Transform _spawnAreaParent;
        [SerializeField] private LineRenderer _lineRenderer;

        [Header("Set Area Name")]
        [SerializeField] private string _areaName = "Spawn Area";

        private bool _drawLines = true;
        private readonly List<LineRenderer> _createdAreas = new();

        /// <summary>
        /// Creates a new spawn area object.
        /// </summary>
        public void CreateSpawnArea()
        {
            LineRenderer newSpawnAreaRenderer = Instantiate(_lineRenderer, _spawnAreaParent);
            newSpawnAreaRenderer.name = _areaName;
            SpawnArea newSpawnArea = newSpawnAreaRenderer.AddComponent<SpawnArea>();
            newSpawnArea.LineRenderer = newSpawnAreaRenderer;

            newSpawnArea.AddUpdateAreaToGridCreated();
            newSpawnArea.UpdateArea();
            _createdAreas.Add(newSpawnAreaRenderer);

            ResourceSpawnAreas newResourceSpawnArea = new() { Area = newSpawnArea };
            _resourceSpawner.SpawnAreas.Add(newResourceSpawnArea);

            RemoveComponents(newSpawnAreaRenderer);
            ClearLine();
        }

        /// <summary>
        /// Toggles the linerenderers visual lines on and off.
        /// </summary>
        public void ToggleLines()
        {
            _drawLines = !_drawLines;

            for (int i = _createdAreas.Count - 1; i >= 0; i--)
            {
                LineRenderer line = _createdAreas[i];
                if (line == null)
                {
                    _createdAreas.Remove(line);
                    continue;
                }

                line.startWidth = _drawLines ? 1 : 0;
                line.endWidth = _drawLines ? 1 : 0;
            }
        }

        /// <summary>
        /// Clears the current spawned SpawnAreas
        /// </summary>
        public void ClearSpawnAreas()
        {
            _resourceSpawner.SpawnAreas.Clear();
            for (int i = _createdAreas.Count - 1; i >= 0; i--)
                DestroyImmediate(_createdAreas[i].gameObject);

            _createdAreas.Clear();
        }

        private void RemoveComponents(LineRenderer newLine)
        {
            DestroyImmediate(newLine.GetComponent<SpawnAreaCreator>());
            DestroyImmediate(newLine.GetComponent<ResourceSpawner>());
        }

        private void ClearLine() => _lineRenderer.positionCount = 0;
    }
}