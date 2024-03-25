using System.Collections.Generic;
using UnityEngine;

namespace Examen.PlayerDatabase
{
    public class PlayerDataFetchterDebug : MonoBehaviour
    {
        [SerializeField] private bool _get = false;
        [SerializeField] private bool _post = false;

        [Header("Get Variables")]
        [SerializeField] private int _getId;

        [Header("Post Variables")]
        [SerializeField] private int _postId;
        [SerializeField] private int _level;
        [SerializeField] private int _exp;
        [SerializeField] private int _buildings;

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
            PlayerDataFetcher.Fetch(_getId);
        }

        private void Post()
        {
            PlayerData playerData = new(_postId, _level, _exp, _buildings);
            PlayerDataFetcher.UpdatePlayerData(playerData);
        }

        private void OnGot(PlayerData data)
        {
            Debug.Log($"Got from player id : {data.id}");
            Debug.Log($"level : {data.level}");
            Debug.Log($"exp : {data.exp}");
            Debug.Log($"builds : {data.buildings}");
        }

        private void OnPosted(PlayerData callback)
        {
            Debug.Log("Posted");
            Debug.Log(callback);
        }
    }
}