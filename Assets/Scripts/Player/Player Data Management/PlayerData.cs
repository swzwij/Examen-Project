namespace Examen.Player.PlayerDataManagement
{
    public struct PlayerData
    {
        private int _exp;

        public readonly int Exp => _exp;

        public PlayerData(int exp) => _exp = exp; 

        /// <summary>
        /// Increases the experience points (exp) by the specified amount.
        /// </summary>
        /// <param name="exp">The amount of experience points to add.</param>
        public void AddExp(int exp) => _exp += exp;
    }
}