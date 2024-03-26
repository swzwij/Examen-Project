using UnityEngine;
using UnityEngine.Events;

namespace MarkUlrich.Health
{
    public class HealthData : MonoBehaviour
    {
        [SerializeField] private float health = 100f;
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
        }

        public void AddHealth(float healthAmount)
        {
            if (isDead || HasMaxHealth) return;

            health += healthAmount;

            onHealthAdded?.Invoke();
            TriggerChangedEvent(HealthEventTypes.AddHealth, healthAmount);
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

        public void Resurrect(float newHealth)
        {
            isDead = false;
            AddHealth(newHealth);
            onResurrected?.Invoke();
            
            TriggerChangedEvent(HealthEventTypes.Resurrect, newHealth);
        }

        public void TakeDamage(float damage)
        {
            if (isDead || _isHit) return;

            _isHit = true;
            health -= damage;
            onDamageTaken?.Invoke();
            TriggerChangedEvent(HealthEventTypes.TakeDamage, damage);
            _isHit = false;
            
            if (health <= 0) Die();
        }

        private void Die()
        {
            health = 0;
            isDead = true;
            onDie?.Invoke();
            TriggerChangedEvent(HealthEventTypes.Die);
        }

        public void Kill()
        {
            Die();
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}