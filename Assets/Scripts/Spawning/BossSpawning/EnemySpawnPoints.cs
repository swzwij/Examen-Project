using System.Collections.Generic;
using UnityEngine;

namespace Examen.Spawning.BossSpawning
{
    public class EnemySpawnPoints : MonoBehaviour
    {
        private Transform _spawnpoint;
        private List<Transform> _waypoints = new();

        public Transform Spawnpoint => _spawnpoint;
        public List<Transform> Waypoints => _waypoints;

        private void Start() => SetWaypoints();

        /// <summary>
        /// Gets the childeren transform and sets the first one as a spawnpoint 
        /// and the rest as waypoints.
        /// </summary>
        public void SetWaypoints()
        {
            _spawnpoint = transform.GetChild(0);

            for (int i = 1; i < transform.childCount; i++)
                _waypoints.Add(transform.GetChild(i));
        }
    }
}