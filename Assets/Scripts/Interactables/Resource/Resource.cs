using Examen.Items;
using Examen.Networking;
using Examen.Pathfinding.Grid;
using Examen.Player.PlayerDataManagement;
using Examen.Poolsystem;
using Examen.Spawning.ResourceSpawning;
using FishNet.Connection;
using FishNet.Object;
using MarkUlrich.Health;
using UnityEngine;

namespace Examen.Interactables.Resource
{
    [RequireComponent(typeof(HealthData))]
    public class Resource : Interactable
    {
        [SerializeField] protected Item p_resourceItem;
        [SerializeField] protected int p_supplyAmount = 1;
        [SerializeField] protected int p_respawnTime;

        protected HealthData p_healthData;
        protected bool p_hasServerStarted;
        protected Cell p_cell;

        public bool IsDead => p_healthData.isDead; 
        public Item ResourceItem => p_resourceItem;
        public SpawnArea SpawnArea { get; set; }
        public Cell Cell { get => p_cell; set => p_cell = value; }

        private void OnEnable()
        {
            if (!p_hasServerStarted || !IsServer)
                return;

            RespawnResource();
        }

        private void Start() => ServerInstance.Instance.OnServerStarted += InitResource;

        /// <summary>
        /// If object isServer, it resurrects this gameobject and calls for the clients to mimic its position and active state.
        /// </summary>
        public virtual void InitResource()
        {
            if (!IsServer)
                return;

            p_hasServerStarted = true;   
            p_healthData = GetComponent<HealthData>();

            p_healthData.onDie.AddListener(StartRespawnTimer);
            p_healthData.onDie.AddListener(GetCell);
            p_healthData.onDie.AddListener(DisableObject);
            p_healthData.onDie.AddListener(UpdateCell);

            p_healthData.onResurrected.AddListener(EnableObject);
            p_healthData.onResurrected.AddListener(UpdateCell);
        }

        /// <summary>
        /// Starts the deathTimer and despawns this object.
        /// </summary>
        public virtual void StartRespawnTimer()
        {
            PoolSystem.Instance.StartRespawnTimer(p_respawnTime, p_resourceItem.Name, transform.parent);
            PoolSystem.Instance.DespawnObject(p_resourceItem.Name, gameObject);
        }

        /// <summary>
        /// Set the object Active.
        /// </summary>
        [ObserversRpc]
        public void EnableObject() => gameObject.SetActive(true);

        /// <summary>
        /// Disables the object.
        /// </summary>
        [ObserversRpc]
        public void DisableObject() => gameObject.SetActive(false);

        /// <summary>
        /// Sets client resource position to server resource position.
        /// </summary>
        /// <param name="newPosition"> the position of the server resource</param>
        [ObserversRpc]
        public virtual void SetNewPosition(Vector3 newPosition) => transform.position = newPosition;


        /// <summary>
        /// Calls all functionalities that need to happen when you are interacting with this Resource
        /// and sends that to the server.
        /// </summary>
        [Server]
        public override void Interact(NetworkConnection connection, float damageAmount = 0)
        {
            PlayInteractingSound();

            ServerInventory.Instance.AddItem(connection, p_resourceItem, p_supplyAmount);
            PlayerDatabase.Instance.AddExp(connection, 1);

            p_healthData.TakeDamage(damageAmount);

            ReceiveInteract();
        }

        /// <summary>
        /// Plays interacting sound.
        /// </summary>
        public override void PlayInteractingSound()
        {
            // Todo: Play interacting sound
        }

        /// <summary>
        /// Calls all functionalities that need to happen when someone else is interacting with this Resource.
        /// </summary>
        [ObserversRpc]
        public virtual void ReceiveInteract()
        {
            // Todo: Play given animation
        }

        [Server]
        protected void GetCell() => Cell = GridSystem.Instance.GetCellFromWorldPosition(transform.position);

        [Server]
        protected void UpdateCell() => SpawnArea.DelayCellUpdate(Cell);

        [Server]
        protected virtual void RespawnResource()
        {
            transform.position = SpawnArea.GetRandomPosition(out p_cell);
            SetNewPosition(transform.position);
            p_healthData.Resurrect(p_healthData.MaxHealth);
        }

        private void OnDestroy() => ServerInstance.Instance.OnServerStarted -= InitResource;
    }
}
