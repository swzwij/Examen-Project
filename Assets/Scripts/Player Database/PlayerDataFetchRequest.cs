namespace Examen.PlayerDatabase
{
    public class PlayerDataFetchRequest : Swzwij.APIManager.APIRequest
    {
        private readonly int _playerId;

        public override string URL => $"fetch_player_data.php?id={_playerId}";

        public PlayerDataFetchRequest(int playerId) => _playerId = playerId;
    }
}