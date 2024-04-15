namespace Examen.Player.PlayerDataManagement
{
    public struct PlayerData
    {
        private int _exp;

        public readonly int Exp => _exp;

        public PlayerData(int exp) 
        { 
            _exp = exp; 
        }

        public void AddExp(int exp) => _exp += exp;
    }
}