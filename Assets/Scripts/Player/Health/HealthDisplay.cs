using UnityEngine;

namespace Examen.Player.Health
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject[] _hearts;

        private float _maxHealth;

        public void InitHealthDisplay(int maxHealth) => _maxHealth = maxHealth;

        public void UpdateHealthDisplay(float health)
        {
            int heartsToShow = Mathf.CeilToInt(health / (_maxHealth / _hearts.Length));
            for (int i = 0; i < _hearts.Length; i++)
            {
                if (i < heartsToShow)
                    _hearts[i].SetActive(true);
                else
                    _hearts[i].SetActive(false);
            }
        }
    }
}