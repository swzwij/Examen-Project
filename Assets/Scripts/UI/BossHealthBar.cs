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

    public void ServerInitialize() => BossHealthData.onDamageTaken.AddListener(CallSetHealth);

    public void ClientInitialize(float bossMaxHealth)
    {
        _healthBar.maxValue = bossMaxHealth;
        _healthBar.value = _healthBar.maxValue;
    }

    private void CallSetHealth() => SetUIHealth(BossHealthData.Health);

    [ObserversRpc]
    private void SetUIHealth(float bossHealth) => _healthBar.value = bossHealth;

}
