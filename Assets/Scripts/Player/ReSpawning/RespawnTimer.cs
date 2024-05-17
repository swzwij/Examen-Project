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

        private HealthData _healthData;

        private void Start() => _healthData = _player.GetComponent<HealthData>();

        private void OnEnable() => StartCoroutine(CountDown());
        private void OnDisable() => StopCoroutine(CountDown());

        private IEnumerator CountDown()
        {
            _currentCount = _startCount;

            for (int i = _currentCount; i > 0; i--)
            {
                _countDown.text = i.ToString();
                yield return new WaitForSeconds(1);
            }

            _healthData.Resurrect(100);
        }
    }
}

