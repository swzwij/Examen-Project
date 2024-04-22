using Examen.Networking;
using FishNet.Object;
using MarkUlrich.Health;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : NetworkBehaviour
{
    [SerializeField] private HealthData _bossHealthData;
    [SerializeField] private bool _damage;

    [SerializeField] private Slider _healthBar;

    private void Start() => ServerInstance.Instance.OnServerStarted += InitializeHealthBar;

    private void Update()
    {
        if (_damage) _bossHealthData.TakeDamage(1);
    }
    private void InitializeHealthBar()
    {
        if (IsClient)
        {
            _healthBar = GetComponent<Slider>();
            _healthBar.maxValue = _bossHealthData.MaxHealth;
        }
        else
        {
            _bossHealthData.onDamageTaken.AddListener(CallSetHealth);
        }
    }


    private void CallSetHealth() => SetUIHealth(_bossHealthData.Health);

    [ObserversRpc]
    private void SetUIHealth(float bossHealth)
    {
        _healthBar.value = bossHealth;
    }
}
