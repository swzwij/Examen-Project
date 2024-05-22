using UnityEngine;

namespace Examen.Player.Health
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject[] _hearts;

        private float _maxHealth;
        
        /// <summary>
        /// Initializes the health display with the maximum health.
        /// </summary>
        /// <param name="maxHealth">The max health.</param>
        public void InitHealthDisplay(float maxHealth) => _maxHealth = maxHealth;

        /// <summary>
        /// Updates the health display based on the current health.
        /// </summary>
        /// <param name="health">The updated health.</param>
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