namespace Examen.Player.PlayerDatabase.Requests
{
    public class PlayerDataFetchRequest : Swzwij.APIManager.APIRequest
    {
        private readonly string _playerId;

        public override string URL => $"fetch_player_data.php?id={_playerId}";

        public PlayerDataFetchRequest(string playerId) => _playerId = playerId;
    }
}