using Examen.Networking;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoints : MonoBehaviour
{
   private Transform _spawnpoint;
   private List<Transform> _waypoints = new();

    public Transform Spawnpoint {  get { return _spawnpoint; } }
    public List<Transform> Waypoints {  get { return _waypoints; } }

    private void Start() => SetWaypoints();

    public void SetWaypoints()
    {
        _spawnpoint = transform.GetChild(0);

        for (int i = 1; i < transform.childCount; i++)
            _waypoints.Add(transform.GetChild(i));
    }
}
