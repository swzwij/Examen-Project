using Examen.Player.PlayerDatabase.Reponses;
using UnityEngine;
using System;

namespace Examen.Player.PlayerDatabase
{
    public class PlayerDataFetchterDebug : MonoBehaviour
    {
        [Header("Fetch")]
        [SerializeField] private bool _sendFetchRequest = false;

        [Header("Update")]
        [SerializeField] private bool _sendUpdateRequest = false;
        [SerializeField] private int _exp;

        private void Update()
        {
            if (_sendFetchRequest)
            {
                _sendFetchRequest = false;
                Get();
            }

            if (_sendUpdateRequest)
            {
                _sendUpdateRequest = false;
                Post();
            }
        }

        private void Get()
        {
            Action<PlayerData> OnFetched = (data) =>
            {
                Debug.Log($"Got from player id : {data.id}");
                Debug.Log($"exp : {data.exp}");
            };

            PlayerDataFetcher.Fetch(OnFetched);
        }

        private void Post()
        {
            Action<PlayerDataUpdateResponse> OnUpdated = (callback) =>
            {
                Debug.Log("Posted");
                Debug.Log(callback);
            };

            PlayerDataFetcher.UpdatePlayerExp(_exp, OnUpdated);
        }
    }
}