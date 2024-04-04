using Examen.Inventory;
using Examen.Items;
using Examen.Poolsystem;
using FishNet.Object;
using MarkUlrich.Health;
using UnityEngine;

namespace Examen.Interactables.Resource
{
    [RequireComponent(typeof(HealthData))]
    public class Resource : NetworkBehaviour, Interactable
    {
        [SerializeField] protected Item p_resourceItem;
        [SerializeField] protected int p_supplyAmount = 1;
        [SerializeField] protected int p_damageAmount = 1;
        [SerializeField] protected int p_respawnTime;

        protected HealthData p_healthData;
        protected bool p_hasHealthData;

        public Item ResourceItem { get => p_resourceItem; }


        /// <summary>
        /// If object isServer, it resurrects this gameobject and calls for the clients to mimic its position and active state.
        /// </summary>
        public virtual void OnEnable()
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
        public virtual void Interact()
        {
            PlayInteractingSound();
            InventorySystem.AddItem(p_resourceItem, p_supplyAmount);

            ServerInteract();
        }

        /// <summary>
        /// Plays interacting sound.
        /// </summary>
        public virtual void PlayInteractingSound()
        {
            // Todo: Play interacting sound
        }

        /// <summary>
        /// Let the player interact to server resource.
        /// </summary>
        [Server]
        public virtual void ServerInteract()
        {
            p_healthData.TakeDamage(p_damageAmount);
            ReceiveInteract();
        }

        /// <summary>
        /// Calls all functionalities that need to happen when someone else is interacting with this Resource.
        /// </summary>
        [ObserversRpc]
        public virtual void ReceiveInteract()
        {
            // Todo: Play given animation
        }
    }
}
