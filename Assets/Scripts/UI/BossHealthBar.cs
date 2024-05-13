using Examen.Poolsystem;
using FishNet.Object;
using MarkUlrich.Health;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : NetworkBehaviour
{
    [SerializeField] private bool _damage;

    [SerializeField] private Slider _healthBar;

    public HealthData BossHealthData { get; set; }
    public Slider HealthSlider => _healthBar;

    private void Update()
    {
        if (_damage) BossHealthData.TakeDamage(1);
    }

    public void ServerInitialize()
    {
        BossHealthData.onDamageTaken.AddListener(CallSetHealth);
        BossHealthData.onDie.AddListener(Despawn);
        BossHealthData.onDie.AddListener(BroadcastDespawn);
    }

    public void ClientInitialize(float bossMaxHealth)
    {
        _healthBar.maxValue = bossMaxHealth;
        _healthBar.value = _healthBar.maxValue;
    }

    private void CallSetHealth() => SetUIHealth(BossHealthData.Health);

    [ObserversRpc]
    private void SetUIHealth(float bossHealth) => _healthBar.value = bossHealth;

    private void Despawn()
    {
        BossHealthData.onDamageTaken.RemoveListener(CallSetHealth);
        BossHealthData.onDie.RemoveListener(Despawn);
        BossHealthData.onDie.RemoveListener(BroadcastDespawn);

        BossSpawningManager.Instance.DespawnBoss(this);
    }

    [ObserversRpc]
    private void BroadcastDespawn() => gameObject.SetActive(false);
}
