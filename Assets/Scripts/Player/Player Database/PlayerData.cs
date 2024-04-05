using System;

namespace Examen.Player.PlayerDatabase
{
    [Serializable]
    public class PlayerData
    {
        public string id;
        public int exp;

        /// <summary>
        /// Represents a player's core data, including their unique ID and experience points (exp).
        /// </summary>
        /// <param name="id">The player's unique identifier.</param>
        /// <param name="exp">The player's current experience points.</param>
        public PlayerData(string id, int exp)
        {
            this.id = id;
            this.exp = exp;
        }
    }
}