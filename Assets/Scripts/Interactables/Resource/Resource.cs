using Examen.Inventory;
using Examen.Items;
using Examen.Networking;
using Examen.Poolsystem;
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
        protected bool p_hasHealthData;

        public Item ResourceItem => p_resourceItem;

        private void Start() => ServerInstance.Instance.OnServerStarted += InitResource;

        /// <summary>
        /// If object isServer, it resurrects this gameobject and calls for the clients to mimic its position and active state.
        /// </summary>
        public virtual void InitResource()
        {
            if (!IsServer)
                return;

            if (!p_hasHealthData)
            {
                p_healthData = GetComponent<HealthData>();
                p_hasHealthData = true;

                p_healthData.onDie.AddListener(StartRespawnTimer);
                p_healthData.onDie.AddListener(DisableObject);

                p_healthData.onResurrected.AddListener(SetObjectActive);
            }

            SetNewPostion(transform.position);

            p_healthData.Resurrect(p_healthData.MaxHealth);

            print("Resource initialized");
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
        public void SetObjectActive() => gameObject.SetActive(true);

        /// <summary>
        /// Disables the object.
        /// </summary>
        [ObserversRpc]
        public void DisableObject() => gameObject.SetActive(false);

        /// <summary>
        /// Sets client resource position to server resource postion.
        /// </summary>
        /// <param name="newPosition"> the postion of the server resource</param>
        [ObserversRpc]
        public virtual void SetNewPostion(Vector3 newPosition) => transform.position = newPosition;

        /// <summary>
        /// Calls all functionalities that need to happen when you are interacting with this Resource
        /// and sends that to the server.
        /// </summary>
        public override void Interact(float damageAmount = 0)
        {
            PlayInteractingSound();
            InventorySystem.AddItem(p_resourceItem, p_supplyAmount);

            RequestServerInteract(damageAmount);
        }

        /// <summary>
        /// Plays interacting sound.
        /// </summary>
        public override void PlayInteractingSound()
        {
            // Todo: Play interacting sound
        }

        [ServerRpc(RequireOwnership = false)]
        private void RequestServerInteract(float damageAmount) => ServerInteract(damageAmount);

        /// <summary>
        /// Let the player interact to server resource.
        /// </summary>
        [Server]
        public virtual void ServerInteract(float damageAmount)
        {
            p_healthData.TakeDamage(damageAmount);
            ReceiveInteract();
            Debug.LogError("Server Interacted with " + name);
        }

        /// <summary>
        /// Calls all functionalities that need to happen when someone else is interacting with this Resource.
        /// </summary>
        [ObserversRpc]
        public virtual void ReceiveInteract()
        {
            // Todo: Play given animation
        }

        private void OnDestroy() => ServerInstance.Instance.OnServerStarted -= InitResource;
    }
}
