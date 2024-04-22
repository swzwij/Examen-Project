using Examen.Pathfinding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Examen.Structure 
{
    public class SpeedRoadStructure : MonoBehaviour
    {
        [SerializeField] private float _speedMultiplier = 1.5f;

        private Dictionary<Collider, PathFollower> _players = new();

        private Collider[] _overlappingColliders = new Collider[16];
        private Collider[] _previousCollides = new Collider[16];

        private void Update()
        {
            Physics.OverlapBoxNonAlloc(gameObject.transform.position, new Vector3(1.5f, 1, 3.5f), _overlappingColliders);
            foreach (var collider in _overlappingColliders)
            {
                //Enter
                if (!_players.ContainsKey(collider))
                    EnterCollider(collider);

                //Exit
                else if (_previousCollides.Length < _overlappingColliders.Length)
                    CheckForExits();
            }

            _previousCollides = _overlappingColliders;
        }

        private void CheckForExits()
        {
            foreach (var player in _players)
            {
                if (!_overlappingColliders.Contains(player.Key))
                {
                    _players.Remove(player.Key);
                    ExitCollider(player.Key);

                    break;
                }
            }
        }

        private void EnterCollider(Collider collider)
        {
            if (collider.gameObject.layer != 3) //Player layer
                return;

            Debug.Log("ENTER COLLIDER");

            PathFollower currentFollower = GetPathFollower(collider);

            MultiplySpeed(currentFollower, out float speed);
            SetPathFollowerSpeed(currentFollower, speed);
        }

        private void ExitCollider(Collider collider)
        {
            if (collider.gameObject.layer != 3) //Player layer
                return;

            Debug.Log("EXIT COLLIDER");

            PathFollower currentFollower = GetPathFollower(collider);

            SetPathFollowerSpeed(currentFollower, currentFollower._baseSpeed);
        }

        private PathFollower GetPathFollower(Collider currentCollider)
        {
            if (!_players.ContainsKey(currentCollider))
                _players.Add(currentCollider, currentCollider.GetComponent<PathFollower>());

            _players.TryGetValue(currentCollider, out PathFollower currentFollower);

            return currentFollower;
        }

        private float MultiplySpeed(PathFollower pathFollower, out float speed) 
            => speed = pathFollower._baseSpeed * _speedMultiplier;

        private void SetPathFollowerSpeed(PathFollower pathFollower, float speed) 
            => pathFollower.Speed = speed;
    }
}