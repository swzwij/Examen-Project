using FishNet.Object;
using MarkUlrich.Health;
using UnityEngine;
using UnityEngine.UI;

namespace Examen.UI
{
    public class EnemyHealthBar : NetworkBehaviour
    {
        [SerializeField] private bool _damage;
        [SerializeField] private Slider _healthBar;

        public HealthData EnemyHealthData { get; set; }
        public Slider HealthSlider => _healthBar;

        private void Update()
        {
            if (!IsServer)
                _healthBar.transform.parent.rotation = Quaternion.LookRotation(Vector3.back);

            if (_damage)
                EnemyHealthData.TakeDamage(1);
        }

        /// <summary>
        /// Sets up the health on the server side
        /// </summary>
        public void ServerInitialize() => EnemyHealthData.onDamageTaken.AddListener(CallSetHealth);

        /// <summary>
        /// Sets up the health on the client side
        /// </summary>
        public void ClientInitialize(float enemyMaxHealth)
        {
            _healthBar.maxValue = enemyMaxHealth;
            _healthBar.value = _healthBar.maxValue;
        }

        private void CallSetHealth() => SetUIHealth(EnemyHealthData.Health);

        [ObserversRpc]
        private void SetUIHealth(float enemyHealth) => _healthBar.value = enemyHealth;

    }
}