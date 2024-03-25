using Newtonsoft.Json;
using Swzwij.APIManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.PlayerDatabase
{
    public static class PlayerDataFetcher
    {
        public static Action<PlayerData> OnDataFetched;
        public static Action<PlayerData> OnDataUpdated;

        public static void Fetch(int id)
        {
            PlayerDataFetchRequest request = new(id);
            APIManager.Instance.GetCall<PlayerData>(request, OnDataRecieved, OnRequestError);
        }

        private static void OnDataRecieved(PlayerData playerData) => OnDataFetched?.Invoke(playerData);

        public static void UpdatePlayerData(PlayerData playerData)
        {
            PlayerDataUpdateRequest request = new(playerData);
            APIManager.Instance.GetCall<PlayerData>(request, OnDataUpdateSuccess, OnRequestError);
        }

        private static void OnDataUpdateSuccess(PlayerData response) => OnDataUpdated?.Invoke(response);

        private static void OnRequestError(APIStatus status) => Debug.LogError("Request Error: " + status);
    }
}