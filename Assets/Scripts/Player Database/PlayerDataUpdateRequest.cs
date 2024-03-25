using System.Collections.Generic;
using UnityEngine;

namespace Examen.PlayerDatabase
{
    public class PlayerDataUpdateRequest : Swzwij.APIManager.APIRequest
    {
        private readonly Dictionary<string, object> _updates;

        public override string URL => "http://localhost/update_player_data.php";

        public PlayerDataUpdateRequest(Dictionary<string, object> updates) => _updates = updates;

        public string GetBody()
        {
            if (_updates == null)
            {
                return null;
            }

            return JsonUtility.ToJson(_updates);
        }
    }
}