using Swzwij.APIManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.PlayerDatabase
{
    public class PlayerDataFetcher : MonoBehaviour
    {
        public Action<PlayerData> OnDataFetched;
        public Action<string> OnDataUpdated;

        public void Fetch(int id)
        {
            PlayerDataFetchRequest request = new(id);
            APIManager.Instance.GetCall<PlayerData>(request, OnDataRecieved, OnRequestError);
        }

        private void OnDataRecieved(PlayerData playerData) => OnDataFetched?.Invoke(playerData);

        public void UpdatePlayerData(int id, Dictionary<string, object> updates)
        {
            PlayerDataUpdateRequest request = new(updates);
            //APIManager.Instance.PostCall(request, request.GetBody(), OnDataUpdateSuccess, OnRequestError);
        }

        private void OnDataUpdateSuccess(string responseMessage) => OnDataUpdated?.Invoke(responseMessage);

        private void OnRequestError(APIStatus status) => Debug.LogError("Request Error: " + status);

    }
}