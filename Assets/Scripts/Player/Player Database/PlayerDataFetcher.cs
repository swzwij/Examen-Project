using Swzwij.APIManager;
using System;
using UnityEngine;
using Examen.Player.PlayerDatabase.Requests;
using Examen.Player.PlayerDatabase.Reponses;

namespace Examen.Player.PlayerDatabase
{
    public static class PlayerDataFetcher
    {
        public static Action<PlayerData> OnDataFetched;
        public static Action<PlayerDataUpdateResponse> OnDataUpdated;

        public static void Fetch(string id)
        {
            PlayerDataFetchRequest request = new(id);
            APIManager.Instance.GetCall<PlayerData>(request, OnDataRecieved, OnRequestError);
        }

        private static void OnDataRecieved(PlayerData playerData) => OnDataFetched?.Invoke(playerData);

        public static void UpdatePlayerData(PlayerData playerData)
        {
            PlayerDataUpdateRequest request = new(playerData);
            APIManager.Instance.GetCall<PlayerDataUpdateResponse>(request, OnDataUpdateSuccess, OnRequestError);
        }

        private static void OnDataUpdateSuccess(PlayerDataUpdateResponse response) => OnDataUpdated?.Invoke(response);

        private static void OnRequestError(APIStatus status) => Debug.LogError("Request Error: " + status);
    }
}