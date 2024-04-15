using UnityEngine;

namespace Examen.Utils
{
    public static class PlayerGUID
    {
        private const string PlayerGuidKey = "PlayerGuid";

        public static string Get
        {
            get
            {
                if (PlayerPrefs.HasKey(PlayerGuidKey))
                    return PlayerPrefs.GetString(PlayerGuidKey);

                string newPlayerGuid = System.Guid.NewGuid().ToString();
                PlayerPrefs.SetString(PlayerGuidKey, newPlayerGuid);
                PlayerPrefs.Save();
                return newPlayerGuid;
            }
        }
    }
}