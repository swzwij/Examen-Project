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
        public Item ResourceItem;

        [SerializeField] protected int p_amountToGive = 1;
        [SerializeField] protected int p_damageAmount = 1;
        [SerializeField] protected int p_deathTime;

        protected HealthData p_healthData;
        protected PoolSystem p_poolSystem;
        protected bool p_hasHealthData;

        /// <summary>
        /// Sets PoolSystem instance.
        /// </summary>
        public virtual void Start() => p_poolSystem = PoolSystem.Instance;

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

                p_healthData.onDie.AddListener(StartDeathTimer);
                p_healthData.onDie.AddListener(ToggleGameobject);

                p_healthData.onResurrected.AddListener(ToggleGameobject);
            }

            SetNewPostion(transform.position);

            p_healthData.Resurrect(p_healthData.MaxHealth);
        }

        /// <summary>
        /// Starts the deathTimer and despawns this object.
        /// </summary>
        public virtual void StartDeathTimer()
        {
            p_poolSystem.StartDeathTimer(p_deathTime, ResourceItem.Name, transform.parent);
            p_poolSystem.DespawnObject(ResourceItem.Name, gameObject);
        }

        /// <summary>
        /// Set the active to the opposite of its current active state.
        /// </summary>
        [ObserversRpc]
        public void ToggleGameobject() => gameObject.SetActive(!gameObject.activeSelf);

        /// <summary>
        /// Sets client resource position to server resource postion.
        /// </summary>
        /// <param name="serverPosition"> the postion of the server resource</param>
        [ObserversRpc]
        public virtual void SetNewPostion(Vector3 serverPosition) => transform.position = serverPosition;

        /// <summary>
        /// Calls all functionalities that need to happen when you are interacting with this Resource
        /// and sends that to the server.
        /// </summary>
        public virtual void Interact()
        {
            PlayInteractingSound();
            InventorySystem.AddItem(ResourceItem, p_amountToGive);

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
        /// Calls all functionalities that need to happen when someone else is interacting with this Resource
        /// </summary>
        [ObserversRpc]
        public virtual void ReceiveInteract()
        {
            // Todo: Play given animation
        }
    }
}
