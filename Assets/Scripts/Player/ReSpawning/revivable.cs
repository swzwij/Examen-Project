using Examen.Pathfinding;
using FishNet.Connection;
using FishNet.Object;
using MarkUlrich.Health;
using System.Collections;
using UnityEngine;

namespace Examen.Player.ReSpawning
{
    [RequireComponent(typeof(HealthData), typeof(Pointer))]
    public class revivable : Interactable
    {
        [HideInInspector] public bool isAlive = true;
        [HideInInspector] public bool isRevivable;

        [SerializeField] private GameObject _deathScreen;
        [SerializeField] private Vector3 _reSpawnLocation;

        private Pointer _pointer;
        private HealthData _healthData;
        private PathFollower _pathFollower;

        private void Start()
        {
            _pointer = GetComponent<Pointer>();
            _healthData = GetComponent<HealthData>();
            _pathFollower = GetComponent<PathFollower>();

            _healthData.onDie.AddListener(OnDie);
            _healthData.onResurrected.AddListener(OnRevive);
        }

        private void Update()
        {
            if (isRevivable)
                OnRevive();
        }

        /// <summary>
        /// Gets interactions from other players to check if this can be revived.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="damageAmount"></param>
        [Server]
        public override void Interact(NetworkConnection connection, float damageAmount = 0)
        {
            if (isAlive)
                return;

            isRevivable = true;
            ReciveReviveState(true);
        }

        /// <summary>
        /// Plays interacting sound.
        /// </summary>
        public override void PlayInteractingSound()
        {
            // To-do: Play interacting sound
        }


        /// <summary>
        /// Re-spawns the player at the _reSpawnLocation and revives them.
        /// </summary>
        public void ForcedRespawn()
        {
            SendPlayerPosition();
            _healthData.Resurrect(100);

            isAlive = true;
            SendAliveState(isAlive);
        }

        private void OnDie()
        {
            _pointer.CanPoint = false;
            _pathFollower.StopPath();

            _deathScreen.gameObject.SetActive(true);

            isAlive = false;
            SendAliveState(isAlive);
        }

        private void OnRevive()
        {
            _pointer.CanPoint = true;
            _deathScreen.gameObject.SetActive(false);

            isRevivable = false;
            isAlive = true;
            SendAliveState(isAlive);
            SendReviveState(false);
        }

        [ServerRpc]
        private void SendAliveState(bool state) => isAlive = state;

        [ServerRpc]
        private void SendReviveState(bool state) => isRevivable = state;

        [ObserversRpc]
        private void ReciveReviveState(bool state) => isRevivable = state;

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
    }
}