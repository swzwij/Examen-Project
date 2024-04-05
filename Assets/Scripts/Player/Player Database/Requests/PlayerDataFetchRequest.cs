namespace Examen.Player.PlayerDatabase.Requests
{
    public class PlayerDataFetchRequest : Swzwij.APIManager.APIRequest
    {
        private readonly string _playerId;

        public override string URL => $"fetch_player_data.php?id={_playerId}";

        /// <summary>
        /// Constructs a request to retrieve player data from the API,
        /// using the specified player ID.
        /// </summary>
        /// <param name="playerId">The ID of the player whose data is to be fetched.</param>
        public PlayerDataFetchRequest(string playerId) => _playerId = playerId;
    }
}