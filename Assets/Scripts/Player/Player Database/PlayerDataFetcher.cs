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
        /// Initiates the process of fetching player data from the database.
        /// </summary>
        public static void Fetch(Action<PlayerData> OnDataRecieved)
        {
            PlayerDataFetchRequest request = new(GetLocalIPv4());
            APIManager.Instance.GetCall<PlayerData>(request, OnDataRecieved, OnRequestError);
        }

        /// <summary>
        /// Initiates an update to the player's experience (exp) on the database.
        /// </summary>
        /// <param name="exp">The new experience value for the player.</param>
        public static void UpdatePlayerExp(int exp, Action<PlayerDataUpdateResponse> OnDataUpdateSuccess = null)
        {
            PlayerData playerData = new(GetLocalIPv4(), exp);
            PlayerDataUpdateRequest request = new(playerData);
            APIManager.Instance.GetCall<PlayerDataUpdateResponse>(request, OnDataUpdateSuccess, OnRequestError);
        }

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