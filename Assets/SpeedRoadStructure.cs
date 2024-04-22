using Examen.Pathfinding;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Structure 
{
    public class SpeedRoadStructure : MonoBehaviour
    {
        [SerializeField] private float _speedMultiplier;

        private Dictionary<GameObject, PathFollower> _players = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != 3) //Player layer
                return;

            PathFollower currentFollower = GetPathFollower(other.gameObject);

            SetMultipliedSpeed(currentFollower, out float speed);
            SetPathFollowerSpeed(currentFollower, speed);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != 3) //Player layer
                return;

            PathFollower currentFollower = GetPathFollower(other.gameObject);

            SetPathFollowerSpeed(currentFollower, currentFollower._baseSpeed);
        }

        private PathFollower GetPathFollower(GameObject currentObject)
        {
            if (!_players.ContainsKey(currentObject))
                _players.Add(currentObject, currentObject.GetComponent<PathFollower>());

            _players.TryGetValue(currentObject, out PathFollower currentFollower);

            return currentFollower;
        }

        private float SetMultipliedSpeed(PathFollower pathFollower, out float speed) 
            => speed = pathFollower._baseSpeed * _speedMultiplier;

        private void SetPathFollowerSpeed(PathFollower pathFollower, float speed) 
            => pathFollower.Speed = speed;
    }
}