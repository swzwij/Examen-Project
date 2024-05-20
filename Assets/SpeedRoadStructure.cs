using Examen.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Examen.Structure
{
    public class SpeedRoadStructure : MonoBehaviour
    {
        [SerializeField] private LayerMask _affectedLayers;
        [SerializeField] private float _speedMultiplier = 1.5f;
        [SerializeField] private Vector3 _colliderSize = new Vector3(5.5f, 1, 15.5f);

        private Dictionary<Collider, PathFollower> _players = new();

        private Collider[] _overlappingColliders = new Collider[16];
        private Collider[] _previousCollides = new Collider[16];

        private void Update()
        {
            _overlappingColliders = new Collider[16];
            Physics.OverlapBoxNonAlloc(gameObject.transform.position, _colliderSize / 2, _overlappingColliders, 
                transform.rotation, _affectedLayers);

            //Enter Check
            foreach (Collider collider in _overlappingColliders)
            {
                if (collider is null)
                    continue;

                if (!_players.ContainsKey(collider))
                    EnterCollider(collider);
            }

            //Exit Check
            if (!CompareEqualColliderArrays(_overlappingColliders, _previousCollides))
                CheckForExits();

            _previousCollides = SetColliderArray(_previousCollides, _overlappingColliders);
        }

        private void EnterCollider(Collider collider)
        {
            if (collider.gameObject.layer != 3) //Player layer
                return;

            PathFollower currentFollower = GetPathFollower(collider, true);

            MultiplySpeed(currentFollower, out float speed);
            SetPathFollowerSpeed(currentFollower, speed);
        }

        private void CheckForExits()
        {
            foreach (var player in _players)
            {
                if (!_overlappingColliders.Contains(player.Key))
                {
                    ExitCollider(player.Key);
                    _players.Remove(player.Key);

                    break;
                }
            }
        }

        private void ExitCollider(Collider collider)
        {
            if (collider.gameObject.layer != 3) //Player layer
                return;

            PathFollower currentFollower = GetPathFollower(collider, false);

            SetPathFollowerSpeed(currentFollower, currentFollower._baseSpeed);
        }

        private PathFollower GetPathFollower(Collider currentCollider, bool saveFollower)
        {
            if (!_players.ContainsKey(currentCollider) && saveFollower)
                _players.Add(currentCollider, currentCollider.gameObject.GetComponent<PathFollower>());

            _players.TryGetValue(currentCollider, out PathFollower currentFollower);

            return currentFollower;
        }

        private bool CompareEqualColliderArrays(Collider[] ColliderArray1, Collider[] ColliderArray2)
        {
            for (int i = 0; i < ColliderArray1.Length; i++)
            {
                if (ColliderArray1[i] != ColliderArray2[i])
                    return false;
            }

            return true;
        }

        private Collider[] SetColliderArray(Collider[] SetArray, Collider[] GetArray)
        {
            for (int i = 0; i < SetArray.Length; i++)
                SetArray[i] = GetArray[i];

            return SetArray;
        }

        private float MultiplySpeed(PathFollower pathFollower, out float speed) 
            => speed = pathFollower._baseSpeed * _speedMultiplier;

        private void SetPathFollowerSpeed(PathFollower pathFollower, float speed) 
            => pathFollower.Speed = speed;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(gameObject.transform.position, _colliderSize);
        }
    }
}