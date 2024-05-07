using MarkUlrich.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Examen.Player.ReSpawning
{
    public class RespawnTimer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _player;
        [SerializeField] private Text _countDown;

        [Header("Settings")]
        [SerializeField] private int _startCount = 9;
        private int _currentCount;

        private revivable _revivable;

        private void Start()
        {
            _revivable = _player.GetComponent<revivable>();
            Debug.Log(_revivable.gameObject);
        }

        private void OnEnable() => StartCoroutine(CountDown());
        private void OnDisable() => StopCoroutine(CountDown());

        IEnumerator CountDown()
        {
            _currentCount = _startCount;

            for (int i = _currentCount; i > 0; i--)
            {
                _countDown.text = i.ToString();
                yield return new WaitForSeconds(1);
            }

            _revivable.ForcedRespawn();
        }
    }
}

