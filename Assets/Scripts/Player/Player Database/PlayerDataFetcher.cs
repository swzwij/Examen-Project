using Examen.Player.PlayerDatabase.Requests;
using Examen.Player.PlayerDatabase.Reponses;
using Swzwij.APIManager;
using System.Linq;
using UnityEngine;
using System.Net;
using System;

namespace Examen.Player.PlayerDatabase
{
    public static class PlayerDataFetcher
    {
        /// <summary>
        /// Callback invoked when player data is successfully fetched from the database.
        /// </summary>
        public static Action<PlayerData> OnDataFetched;

        /// <summary>
        /// Callback invoked when an update to the player's data is successful.
        /// </summary>
        public static Action<PlayerDataUpdateResponse> OnDataUpdated;

        /// <summary>
        /// Initiates the process of fetching player data from the database.
        /// </summary>
        public static void Fetch()
        {
            PlayerDataFetchRequest request = new(GetLocalIPv4());
            APIManager.Instance.GetCall<PlayerData>(request, OnDataRecieved, OnRequestError);
        }

        /// <summary>
        /// Initiates an update to the player's experience (exp) on the database.
        /// </summary>
        /// <param name="exp">The new experience value for the player.</param>
        public static void UpdatePlayerExp(int exp)
        {
            PlayerData playerData = new(GetLocalIPv4(), exp);
            PlayerDataUpdateRequest request = new(playerData);
            APIManager.Instance.GetCall<PlayerDataUpdateResponse>(request, OnDataUpdateSuccess, OnRequestError);
        }

        private static void OnDataRecieved(PlayerData playerData) => OnDataFetched?.Invoke(playerData);

        private static void OnDataUpdateSuccess(PlayerDataUpdateResponse response) => OnDataUpdated?.Invoke(response);

        private static void OnRequestError(APIStatus status) => Debug.LogError("Request Error: " + status);

        private static string GetLocalIPv4()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First
            (
                adress => adress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork
            ).ToString();
        }
    }
}