using System.Collections;
using System.Collections.Generic;
using MarkUlrich.Health;
using UnityEngine;

namespace Examen.Player.Health
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private HealthData _health;
        [SerializeField] private GameObject[] _hearts;

        private void OnEnable() => _health.onDamageTaken.AddListener(UpdateHealthDisplay);

        private void OnDisable() => _health.onDamageTaken.RemoveListener(UpdateHealthDisplay);

        private void UpdateHealthDisplay()
        {
            int heartsToShow = Mathf.CeilToInt(_health.Health / 33.4f);

            for (int i = 0; i < _hearts.Length; i++)
            {
                if (i < heartsToShow)
                    _hearts[i].SetActive(true);
                else
                    _hearts[i].SetActive(false);
            }
        }
    }
}