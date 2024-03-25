namespace Examen.PlayerDatabase
{
    public class PlayerDataUpdateRequest : Swzwij.APIManager.APIRequest
    {
        private readonly PlayerData _playerData;

        public override string URL => $"update_player_data.php?id={_playerData.id}&level={_playerData.level}&exp={_playerData.exp}&buildings={_playerData.buildings}";

        public PlayerDataUpdateRequest(PlayerData data) => _playerData = data;
    }
}