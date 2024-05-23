using Examen.Player.Health;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Events;

namespace MarkUlrich.Health
{
    public class HealthData : NetworkBehaviour
    {
        [SerializeField] private float health = 100f;
        [SerializeField] private HealthDisplay _healthDisplay;
        private float _maxHealth;

        public UnityEvent<HealthEvent> onHealthChanged = new UnityEvent<HealthEvent>();
        public UnityEvent onHealthAdded = new UnityEvent();
        public UnityEvent onDamageTaken = new UnityEvent();
        public UnityEvent onDie = new UnityEvent();
        public UnityEvent onResurrected = new UnityEvent();

        public float Health => health;
        public float MaxHealth => _maxHealth;
        public bool HasMaxHealth => health >= _maxHealth;

        private bool _isHit;
        [HideInInspector] public bool isDead;

        private void Start()
        {
            InitHealth();
        }

        private void InitHealth()
        {
            _maxHealth = health;
            if (_healthDisplay != null)
                _healthDisplay.InitHealthDisplay((int)_maxHealth);
        }

        [ServerRpc(RequireOwnership = false, RunLocally = true)]
        public void AddHealth(float healthAmount)
        {
            if (isDead || HasMaxHealth) return;

            health += healthAmount;
            BroadcastAddHealth(healthAmount);

            onHealthAdded?.Invoke();

            TriggerChangedEvent(HealthEventTypes.AddHealth, healthAmount);
        }

        [ObserversRpc]
        private void BroadcastAddHealth(float healthAmount)
        {
            if (!IsOwner)
                return;

            health += healthAmount;

            if (_healthDisplay != null)
                _healthDisplay.UpdateHealthDisplay(health);
        }

        private void TriggerChangedEvent(HealthEventTypes type, float healthDelta = 0)
        {
            var healthEvent = new HealthEvent()
            {
                type = type,
                target = gameObject,
                currenthealth = Health,
                healthDelta = healthDelta,
                maxHealth = MaxHealth
            };
            onHealthChanged?.Invoke(healthEvent);
        }

        [ServerRpc(RequireOwnership = false, RunLocally = true)]
        public void Resurrect(float newHealth)
        {
            isDead = false;
            AddHealth(newHealth);
            onResurrected?.Invoke();

            TriggerChangedEvent(HealthEventTypes.Resurrect, newHealth);

            BroadcastResurrect(newHealth, isDead);
        }

        [ObserversRpc]
        private void BroadcastResurrect(float newHealth, bool newIsDead)
        {
            if (!IsOwner)
                return;

            isDead = newIsDead;
            health = newHealth;
            
            if (_healthDisplay != null)
                _healthDisplay.UpdateHealthDisplay(health);
        }

        [ServerRpc(RequireOwnership = false, RunLocally = true)]
        public void TakeDamage(float damage)
        {
            if (isDead || _isHit) return;

            _isHit = true;
            BroadcastHit(_isHit);
            health -= damage;
            BroadcastTakeDamage(damage);
            onDamageTaken?.Invoke();
            TriggerChangedEvent(HealthEventTypes.TakeDamage, damage);
            _isHit = false;
            BroadcastHit(_isHit);

            if (health <= 0) Die();
        }

        [ObserversRpc]
        private void BroadcastTakeDamage(float damage)
        {
            if (!IsOwner)
                return;

            health -= damage;

            
            if (_healthDisplay != null)
                _healthDisplay.UpdateHealthDisplay(health);
        }

        [ObserversRpc]
        private void BroadcastHit(bool isHit)
        {
            if (!IsOwner)
                return;

            _isHit = isHit;
        }

        [Server]
        private void Die()
        {
            health = 0;
            isDead = true;
            onDie?.Invoke();
            TriggerChangedEvent(HealthEventTypes.Die);

            BroadcastDie(health, isDead);
        }

        [ObserversRpc]
        private void BroadcastDie(float newHealth, bool newIsDead)
        {
            if (!IsOwner)
                return;

            health = newHealth;
            isDead = newIsDead;
        }

        public void Kill()
        {
            Die();
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
            BroadcastDestroySelf();
        }

        [ObserversRpc]
        private void BroadcastDestroySelf()
        {
            if (!IsOwner)
                return;

            DestroySelf();
        }
    }
}
