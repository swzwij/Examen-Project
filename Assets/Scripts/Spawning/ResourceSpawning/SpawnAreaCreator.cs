using System.Collections.Generic;
using Examen.Interactables.Resource;
using Examen.Pathfinding.Grid;
using Examen.Spawning.ResourceSpawning;
using Examen.Spawning.ResourceSpawning.Structs;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer), typeof(ResourceSpawner))]
public class SpawnAreaCreator : MonoBehaviour
{
    [SerializeField] private ResourceSpawner _resourceSpawner;
    [SerializeField] private Transform _spawnAreaParent;
    [SerializeField] private LineRenderer _lineRenderer;

    [Header("Set Area Name")]
    [SerializeField] private string _areaName = "Spawn Area";

    private List<LineRenderer> _createdAreas = new();
    private bool _drawLines = true;

    public void CreateSpawnArea()
    {
        LineRenderer newSpawnAreaRenderer = Instantiate(_lineRenderer, _spawnAreaParent);
        newSpawnAreaRenderer.name = _areaName;
        SpawnArea newSpawnArea = newSpawnAreaRenderer.AddComponent<SpawnArea>();
        newSpawnArea.LineRenderer = newSpawnAreaRenderer;
        newSpawnArea.AddAction();
        newSpawnArea.UpdateArea();

        _createdAreas.Add(newSpawnAreaRenderer);
        ResourceSpawnAreas newResourceSpawnArea = new() { Area = newSpawnArea };
        _resourceSpawner.SpawnAreas.Add(newResourceSpawnArea);
        
        RemoveComponents(newSpawnAreaRenderer);
        ClearLine();
    }

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

            line.enabled = _drawLines;
        }
    }

    private void RemoveComponents(LineRenderer newLine)
    {
        DestroyImmediate(newLine.GetComponent<SpawnAreaCreator>());
        DestroyImmediate(newLine.GetComponent<ResourceSpawner>());
    }

    private void ClearLine() => _lineRenderer.positionCount = 0;

    public void ClearSpawnAreas()
    {
        _resourceSpawner.SpawnAreas.Clear();
        for (int i = _createdAreas.Count - 1; i >= 0; i--)
            DestroyImmediate(_createdAreas[i].gameObject);

        _createdAreas.Clear();
    }
}
