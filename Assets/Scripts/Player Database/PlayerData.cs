using System;

namespace Examen.PlayerDatabase
{
    [Serializable]
    public class PlayerData
    {
        public int id;
        public int level;
        public int exp;
        public int buildings;

        public PlayerData(int id, int level, int exp, int buildings)
        {
            this.id = id;
            this.level = level;
            this.exp = exp;
            this.buildings = buildings;
        }
    }
}