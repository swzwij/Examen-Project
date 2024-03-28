using System.Collections.Generic;
using Examen.Pathfinding;
using UnityEngine;

public class WaypointFollower : PathFollower
{
    private Transform _waypointsParent;
    [SerializeField] private List<Transform> _waypoints = new();
    private int _currentWaypointIndex = 0;

    protected override void Start()
    {
        base.Start();

        _waypointsParent = new GameObject().transform;
        _waypointsParent.name = $"{gameObject.name} - Waypoints";
        
        _waypoints.Clear();

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            _waypoints.Add(transform.GetChild(i));
            transform.GetChild(i).SetParent(_waypointsParent);
        }

        _waypoints.Reverse();

        if (_waypoints.Count == 0)
            return;
        
        StartPath(_waypoints[_currentWaypointIndex].position);
        OnPathCompleted += GetNextWaypoint;
    }

    protected void GetNextWaypoint()
    {
        _currentWaypointIndex++;
        if (_currentWaypointIndex >= _waypoints.Count)
            return;
        
        StartPath(_waypoints[_currentWaypointIndex].position);
    }
}
