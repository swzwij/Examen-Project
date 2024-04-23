using Examen.Poolsystem;
using MarkUlrich.Health;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BossSpawningManager : MonoBehaviour
{
    [Header("Boss Spawning")]
    [SerializeField] private int _bossDownTimer = 60;
    [SerializeField] private HealthData _bossPrefab;

    [Header("Boss UI")]
    [SerializeField] private List<Slider> _sliders;

    private Dictionary<GameObject, Slider> _currentActiveBossSliders = new();

    private void Start() => CreateBoss();

    private void CreateBoss()
    {
        if (_currentActiveBossSliders.Count >= 3)
        {
            Debug.LogError("Cant Spawn more bosses");
            return;
        }

        HealthData healthData = Instantiate(_bossPrefab);

        PoolSystem.Instance.AddActiveObject(healthData.name, healthData.gameObject);
        healthData.onDie.AddListener(() => StartNextSpawnTimer(healthData.gameObject));

        AddSlider(healthData.gameObject);
    }

    private void AddSlider(GameObject bossObject)
    {
        Slider newSlider = _sliders[_currentActiveBossSliders.Count + 1];

        newSlider.enabled = true;
        _currentActiveBossSliders.Add(bossObject, newSlider);    
    }

    private void StartNextSpawnTimer(GameObject bossObject)
    {
        if(_currentActiveBossSliders.TryGetValue(bossObject, out Slider slider))
        {
            slider.enabled = false;
            _currentActiveBossSliders.Remove(bossObject);
        }

        PoolSystem.Instance.StartRespawnTimer(_bossDownTimer, bossObject.name, bossObject.transform.parent); // todo: remove this line when we have more bosses
        PoolSystem.Instance.DespawnObject(bossObject.name, bossObject);
    }
}