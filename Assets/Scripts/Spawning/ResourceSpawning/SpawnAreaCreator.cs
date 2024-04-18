using System.Collections.Generic;
using Examen.Pathfinding.Grid;
using Examen.Spawning.ResourceSpawning;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SpawnAreaCreator : MonoBehaviour
{
    [SerializeField] private Transform _spawnAreaParent;
    [SerializeField] private LineRenderer _lineRenderer;

    private List<LineRenderer> _createdAreas = new();
    private bool _drawLines = true;

    public void CreateSpawnArea()
    {
        LineRenderer newSpawnAreaRenderer = Instantiate(_lineRenderer, _spawnAreaParent);
        SpawnArea newSpawnArea = newSpawnAreaRenderer.AddComponent<SpawnArea>();
        newSpawnArea.LineRenderer = newSpawnAreaRenderer;
        newSpawnArea.UpdateArea();

        _createdAreas.Add(newSpawnAreaRenderer);
        
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
}
