using UnityEngine;
using UnityEngine.UI;

namespace Examen.Streak
{
    public class StreakDisplay : MonoBehaviour
    {
        [SerializeField] private Text _streakText;

        private void OnEnable() => StreakCounter.Instance.OnStreakChanged += UpdateStreakDisplay;

        private void OnDisable() => StreakCounter.Instance.OnStreakChanged -= UpdateStreakDisplay;

        private void UpdateStreakDisplay(int streak) => _streakText.text = $"{streak}";
    }
}