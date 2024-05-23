using Examen.Networking;
using Examen.Spawning.BossSpawning;
using FishNet.Object;
using MarkUlrich.Health;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthData))]
public class HeartOfTheVillage : NetworkBehaviour
{
    [SerializeField] private Material _cristalMaterial;
    [SerializeField] private Color _damageColor;
    [SerializeField] private float _regenTimer = 1f;

    private Color _originalColor;
    private HealthData _healthData;
    private float _maxHealth;

    private void Start()
    {
        ServerInstance.Instance.OnServerStarted += InitialiseHeart;
        _healthData = GetComponent<HealthData>();
    }

    private void InitialiseHeart()
    {
        _originalColor = _cristalMaterial.color;
        _healthData.onDie.AddListener(ResetLevel);
        _healthData.onDamageTaken.AddListener(GetDamaged);

        _maxHealth = _healthData.MaxHealth;
    }

    private void ResetLevel()
    {
        EnemySpawner.Instance.DespawnEnemies();
        //TODO: reset streak to 0
        StartCoroutine(Regenerate());
    }

    private IEnumerator Regenerate()
    {
        float time = 0f;
        while (time < _regenTimer)
        {
            time += Time.deltaTime;
            _cristalMaterial.color = Color.Lerp(_damageColor, _originalColor, time / _regenTimer);
            BroadcastNewColor(_cristalMaterial.color);
            yield return null;
        }

        _healthData.Resurrect(_maxHealth);
    }

    private void GetDamaged()
    {
        float multiplier = 1 - _healthData.Health / _healthData.MaxHealth;
        _cristalMaterial.color = Color.Lerp(_cristalMaterial.color, _damageColor, multiplier);
        BroadcastNewColor(_cristalMaterial.color);
    }

    [ObserversRpc]
    private void BroadcastNewColor(Color newColor) => _cristalMaterial.color = newColor;


    private void OnDestroy()
    {
        _cristalMaterial.color = _originalColor;
    }

}
