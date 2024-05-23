using Examen.Networking;
using Examen.Spawning.BossSpawning;
using FishNet.Object;
using MarkUlrich.Health;
using System;
using System.Collections;
using UnityEngine;

namespace Examen.Structures
{
    [RequireComponent(typeof(HealthData))]
    public class HeartOfTheVillage : NetworkBehaviour
    {
        [SerializeField] private Material _crystalMaterial;
        [SerializeField] private Color _damageColor;
        [SerializeField] private float _regenTimer = 1f;

        private Color _originalColor;
        private HealthData _healthData;
        private float _maxHealth;

        public Action OnResetLevel;

        private void Start()
        {
            ServerInstance.Instance.OnServerStarted += InitialiseHeart;
            _healthData = GetComponent<HealthData>();
        }

        private void InitialiseHeart()
        {
            _originalColor = _crystalMaterial.color;
            _healthData.onDie.AddListener(ResetLevel);
            _healthData.onDamageTaken.AddListener(GetDamaged);

            _maxHealth = _healthData.MaxHealth;
        }

        private void ResetLevel()
        {
            EnemySpawner.Instance.DespawnEnemies();
            OnResetLevel?.Invoke();
            StartCoroutine(Regenerate());
        }

        private IEnumerator Regenerate()
        {
            float time = 0f;
            while (time < _regenTimer)
            {
                time += Time.deltaTime;
                _crystalMaterial.color = Color.Lerp(_damageColor, _originalColor, time / _regenTimer);
                BroadcastNewColor(_crystalMaterial.color);
                yield return null;
            }

            _healthData.Resurrect(_maxHealth);
        }

        private void GetDamaged()
        {
            float multiplier = 1 - _healthData.Health / _healthData.MaxHealth;
            _crystalMaterial.color = Color.Lerp(_crystalMaterial.color, _damageColor, multiplier);
            BroadcastNewColor(_crystalMaterial.color);
        }

        [ObserversRpc]
        private void BroadcastNewColor(Color newColor) => _crystalMaterial.color = newColor;


        private void OnDestroy() => _crystalMaterial.color = _originalColor;
    }
}