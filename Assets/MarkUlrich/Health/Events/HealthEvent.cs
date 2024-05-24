using UnityEngine;

namespace MarkUlrich.Health
{
    public class HealthEvent
    {
        public GameObject target;
        public HealthEventTypes type;
        public float healthDelta;
        public float currenthealth;
        public float maxHealth;
    }
}
