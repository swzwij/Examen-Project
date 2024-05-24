using System;
using Examen.Spawning.BossSpawning;
using FishNet.Object;
using MarkUlrich.Utils;

namespace Examen.Streak
{
    public class StreakCounter : NetworkedSingletonInstance<StreakCounter>
    {
        private int _currentStreak;

        public int CurrentStreak => _currentStreak;

        public Action<int> OnStreakChanged;

        protected override void OnEnable()
        {
            base.OnEnable();
            EnemySpawner.Instance.OnEnemyDefeated += IncreaseStreak;
        }

        private void OnDisable() => EnemySpawner.Instance.OnEnemyDefeated -= IncreaseStreak;

        /// <summary>
        /// Increases the current streak by 1.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void IncreaseStreak()
        {
            _currentStreak++;
            BroadCastStreak(_currentStreak);
        }

        /// <summary>
        /// Resets the current streak to 0.
        /// </summary>
        [ServerRpc(RequireOwnership = false)]
        public void ResetStreak()
        {
            _currentStreak = 0;
            BroadCastStreak(_currentStreak);
        }

        /// <summary>
        /// Broadcasts the current streak to all clients.
        /// </summary>
        /// <param name="streak">The current streak.</param>
        [ObserversRpc]
        private void BroadCastStreak(int streak)
        {
            _currentStreak = streak;
            OnStreakChanged?.Invoke(streak);
        }
    }
}