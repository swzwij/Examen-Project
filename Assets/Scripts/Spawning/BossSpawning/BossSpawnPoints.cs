using Examen.Networking;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnPoints : MonoBehaviour
{
    [SerializeField] private List<Transform> _waypoints = new();

    public List<Transform> Waypoints {  get { return _waypoints; } }

    private void Start() => SetWaypoints();

    public void SetWaypoints()
    {
        for (int i = 0; i < transform.childCount; i++)
            _waypoints.Add(transform.GetChild(i));
    }
}
