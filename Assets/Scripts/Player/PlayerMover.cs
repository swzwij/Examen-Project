using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Examen.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private LayerMask _obstacleLayers;

        [SerializeField] private float _knockbackDistance;
        [SerializeField][Range(0.1f, 0.9f)] private float _midPointOffset = 0.5f;

        [SerializeField] private float _rayDistance = 2f; 

        private void StartMoving(GameObject attacker) => StartCoroutine(MoveacrossCurve(attacker.transform.position));

        private IEnumerator MoveacrossCurve(Vector3 attackPosition) 
        {
            Vector3 startPoint = transform.position;

            Vector3 attackDiraction = attackPosition - startPoint.normalized;
            Vector3 endPoint = attackDiraction * _knockbackDistance;

            Vector3 mindPoint = _midPointOffset * Vector3.Normalize(endPoint - startPoint) + startPoint;
            while (true) 
            {
                float count = 0;
                for (float i = 0; i < count; i += 1.0f * Time.deltaTime)
                {
                    Vector3 m1 = Vector3.Lerp(startPoint, mindPoint, count);
                    Vector3 m2 = Vector3.Lerp(mindPoint, endPoint, count);
                    transform.position = Vector3.Lerp(m1, m2, count);
                }
            }
        }
    }
}
