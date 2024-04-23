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

    private void Start()
    {
        if(IsServer)
            ServerInstance.Instance.OnServerStarted += ServerInitialize;
        else
            ClientInitialize();
    }

    private void Update()
    {
        if (_damage) _bossHealthData.TakeDamage(1);
    }

    [Server]
    private void ServerInitialize() => _bossHealthData.onDamageTaken.AddListener(CallSetHealth);

     private void ClientInitialize()
     {
        _healthBar = GetComponent<Slider>();
        _healthBar.maxValue = _bossHealthData.MaxHealth;
        _healthBar.value = _healthBar.maxValue;
     }

    private void CallSetHealth() => SetUIHealth(_bossHealthData.Health);

    [ObserversRpc]
    private void SetUIHealth(float bossHealth) => _healthBar.value = bossHealth;
}
