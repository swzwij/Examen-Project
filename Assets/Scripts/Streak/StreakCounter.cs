using System;
using FishNet.Object;
using MarkUlrich.Utils;

namespace Examen.Streak
{
    public class StreakCounter : NetworkedSingletonInstance<StreakCounter>
    {
        public bool plus;

        private void Update()
        {
            if (plus)
            {
                IncreaseStreak();
                plus = false;
            }
        }

        private int _currentStreak;

        public int CurrentStreak => _currentStreak;

        public Action<int> OnStreakChanged;

        protected override void OnEnable()
        {
            base.OnEnable();
            // Listen to boss defeated event
        }

        private void OnDisable()
        {
            // Unlisten to boss defeated event
        }

        [ServerRpc(RequireOwnership = false)]
        public void IncreaseStreak()
        {
            _currentStreak++;
            BroadCastStreak(_currentStreak);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ResetStreak()
        {
            _currentStreak = 0;
            BroadCastStreak(_currentStreak);
        }

        [ObserversRpc]
        private void BroadCastStreak(int streak)
        {
            _currentStreak = streak;
            OnStreakChanged?.Invoke(streak);
        }
    }
}