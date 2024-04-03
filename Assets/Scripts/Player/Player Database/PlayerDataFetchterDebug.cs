using Examen.Player.PlayerDatabase.Reponses;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Player.PlayerDatabase
{
    public class PlayerDataFetchterDebug : MonoBehaviour
    {
        [SerializeField] private bool _get = false;
        [SerializeField] private bool _post = false;

        [Header("Get Variables")]
        [SerializeField] private string _getId;

        [Header("Post Variables")]
        [SerializeField] private int _exp;

        private void OnEnable()
        {
            PlayerDataFetcher.OnDataFetched += OnGot;
            PlayerDataFetcher.OnDataUpdated += OnPosted;
        }

        private void OnDisable()
        {
            PlayerDataFetcher.OnDataFetched -= OnGot;
            PlayerDataFetcher.OnDataUpdated -= OnPosted;
        }

        private void Update()
        {
            if(_get)
            {
                _get = false;
                Get();
            }

            if(_post)
            {
                _post = false;
                Post();
            }
        }

        private void Get()
        {
            PlayerDataFetcher.Fetch();
        }

        private void Post()
        {
            PlayerDataFetcher.UpdatePlayerExp(_exp);
        }

        private void OnGot(PlayerData data)
        {
            Debug.Log($"Got from player id : {data.id}");
            Debug.Log($"exp : {data.exp}");
        }

        private void OnPosted(PlayerDataUpdateResponse callback)
        {
            Debug.Log("Posted");
            Debug.Log(callback);
        }
    }
}