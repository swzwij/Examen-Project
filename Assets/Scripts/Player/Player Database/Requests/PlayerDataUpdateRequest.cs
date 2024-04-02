namespace Examen.Player.PlayerDatabase.Requests
{
    public class PlayerDataUpdateRequest : Swzwij.APIManager.APIRequest
    {
        private readonly PlayerData _playerData;

        public override string URL => $"update_player_data.php?id={_playerData.id}&exp={_playerData.exp}";

        public PlayerDataUpdateRequest(PlayerData data) => _playerData = data;
    }
}