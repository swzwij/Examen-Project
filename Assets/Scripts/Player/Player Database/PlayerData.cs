using System;

namespace Examen.Player.PlayerDatabase
{
    [Serializable]
    public class PlayerData
    {
        public string id;
        public int exp;

        public PlayerData(string id, int exp)
        {
            this.id = id;
            this.exp = exp;
        }
    }
}