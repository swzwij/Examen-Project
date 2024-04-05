namespace Examen.Player.PlayerDatabase.Requests
{
    public class PlayerDataUpdateRequest : Swzwij.APIManager.APIRequest
    {
        private readonly PlayerData _playerData;

        public override string URL => $"update_player_data.php?id={_playerData.id}&exp={_playerData.exp}";

        /// <summary>
        /// Constructs a request to update player data on the server, using 
        /// the provided PlayerData object.
        /// </summary>
        /// <param name="data">The PlayerData object containing the updated player information.</param>
        public PlayerDataUpdateRequest(PlayerData data) => _playerData = data;
    }
}