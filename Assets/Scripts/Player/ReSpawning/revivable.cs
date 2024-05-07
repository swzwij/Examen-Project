using Examen.Pathfinding;
using Examen.Player;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using MarkUlrich.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Player.ReSpawning
{
    [RequireComponent(typeof(HealthData), typeof(Pointer))]
    public class revivable : Interactable
    {
        [SerializeField] private GameObject _deathScreen;
        [SerializeField] private Vector3 _reSpawnLocation;

        private HealthData _healthData;
        private Pointer _pointer;
        private PathFollower _pathFollower;

        private void Start()
        {
            _pointer = GetComponent<Pointer>();
            _healthData = GetComponent<HealthData>();
            _pathFollower = GetComponent<PathFollower>();

            _healthData.onDie.AddListener(OnDie);
            _healthData.onResurrected.AddListener(OnRevive);

            StartCoroutine(TEMP());
        }

        IEnumerator TEMP()
        {
            yield return new WaitForSeconds(2);
            _healthData.Kill();
        }

        private void OnDie()
        {
            _pointer.CanPoint = false;
            _pathFollower.StopPath();

            _deathScreen.gameObject.SetActive(true);
        }

        private void OnRevive()
        {
            _pointer.CanPoint = true;
            _deathScreen.gameObject.SetActive(false);
        }


        public void ForcedRespawn()
        {
            SendPlayerPosition();
            _healthData.Resurrect(100);
        }

        [ServerRpc]
        private void SendPlayerPosition() => ProcessPosition(_reSpawnLocation);

        [Server]
        private void ProcessPosition(Vector3 _reSpawnLocation)
        {
            Vector3 newPosition = _reSpawnLocation;
            transform.position = newPosition;

            BroadcastPosition(newPosition);
        }

        [ObserversRpc]
        private void BroadcastPosition(Vector3 newPosition) => transform.position = newPosition;

        [Server]
        public override void Interact(NetworkConnection connection, float damageAmount = 0)
        {
            Debug.Log("INTERACTING WITH KNOCKED-OUT PLAYER");
        }

        /// <summary>
        /// Plays interacting sound.
        /// </summary>
        public override void PlayInteractingSound()
        {
            // Todo: Play interacting sound
        }

    }
}