using Examen.Poolsystem;
using FishNet.Object;
using MarkUlrich.Health;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawningManager : NetworkBehaviour
{
    [Header("Boss Spawning")]
    [SerializeField] private int _bossDownTimer = 60;
    [SerializeField] private HealthData _bossPrefab;

    [Header("Boss UI")]
    [SerializeField] private List<BossHealthBar> _sliders;

    private Dictionary<GameObject, BossHealthBar> _currentActiveBossSliders = new();

    private void Start() => ServerInstance.Instance.OnServerStarted += CreateBoss;

    [Server]
    private void CreateBoss()
    {
        HealthData healthData = Instantiate(_bossPrefab);

        PoolSystem.Instance.AddActiveObject(healthData.name, healthData.gameObject);
        healthData.onDie.AddListener(() => StartNextSpawnTimer(healthData.gameObject));

        AddSlider(healthData);
    }


    private void AddSlider(HealthData bossHealth)
    {
        if (_currentActiveBossSliders.Count >= 3)
        {
            Debug.LogError("Cant add more sliders");
            return;
        }

        BossHealthBar healthBar = _sliders[_currentActiveBossSliders.Count + 1];

        healthBar.BossHealthData = bossHealth;
        healthBar.HealthSlider.enabled = true;
        _currentActiveBossSliders.Add(bossHealth.gameObject, _sliders[_currentActiveBossSliders.Count + 1]);    
    }

    private void StartNextSpawnTimer(GameObject bossObject)
    {
        if(_currentActiveBossSliders.TryGetValue(bossObject, out BossHealthBar healthBar))
        {
            healthBar.HealthSlider.enabled = false;
            _currentActiveBossSliders.Remove(bossObject);
        }

        PoolSystem.Instance.StartRespawnTimer(_bossDownTimer, bossObject.name, bossObject.transform.parent); // todo: remove this line when we have more bosses
        PoolSystem.Instance.DespawnObject(bossObject.name, bossObject);
    }
}