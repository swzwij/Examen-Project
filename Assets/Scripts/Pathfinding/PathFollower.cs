using System.Collections;
using System.Collections.Generic;
using Examen.Pathfinding.Grid;
using Examen.Player;
using UnityEngine;

namespace Examen.Pathfinding
{
    public class Pathfollower : MonoBehaviour
    {
        [SerializeField] private float speed = 5f;
        [SerializeField] private Color pathColor = Color.red;
        [SerializeField] private bool canMove;

        private Pathfinder _pathfinder;
        private Pointer _pointer;
        private List<Node> _currentPath = new();
        private int currentPathIndex = 0;
        private Coroutine _followPathCoroutine;

        private void Start()
        {
            _pathfinder = GetComponent<Pathfinder>();
            _pointer = GetComponent<Pointer>();

            _pointer.OnPointedAtPosition += StartPath;
        }

        public void StartPath(Vector3 target)
        {
            if (_followPathCoroutine != null)
                StopCoroutine(_followPathCoroutine);

            Vector3 startPosition = transform.position;
            _currentPath = _pathfinder.FindPath(startPosition, target);
            currentPathIndex = 0;

            if (_currentPath != null && _currentPath.Count > 0)
                _followPathCoroutine = StartCoroutine(FollowPath());
        }

        private IEnumerator FollowPath()
        {
            while (currentPathIndex < _currentPath.Count)
            {
                Vector3 currentWaypoint = _currentPath[currentPathIndex].position;

                while (Vector3.Distance(transform.position, currentWaypoint) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                    yield return null;
                }

                currentPathIndex++;
                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            if (_currentPath != null)
            {
                Gizmos.color = pathColor;

                for (int i = currentPathIndex; i < _currentPath.Count - 1; i++)
                {
                    Gizmos.DrawLine(_currentPath[i].position, _currentPath[i + 1].position);
                }
            }
        }
    }
}
