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
        [HideInInspector] public HealthData HealthData;
        [HideInInspector] public PoolSystem poolSystem;
        [HideInInspector] public bool HasHealthData;

        public Item ResourceItem;
        public int AmountToGive = 1;
        public int DeathTime;

        public const int DamageAmount = 1;

        /// <summary>
        /// Sets PoolSystem instance.
        /// </summary>
        public virtual void Start()
        {
            poolSystem = PoolSystem.Instance;
        }

        /// <summary>
        /// If object isServer, it resurrects this gameobject and calls for the clients to mimic its position and active state
        /// </summary>
        public virtual void OnEnable()
        {
            if (!IsServer)
                return;

            if (!HasHealthData)
            {
                HealthData = GetComponent<HealthData>();
                HasHealthData = true;

                HealthData.onDie.AddListener(StartDeathTimer);
                HealthData.onDie.AddListener(ToggleGameobject);

                HealthData.onResurrected.AddListener(ToggleGameobject);
            }

            SetNewPostion(transform.position);

            HealthData.Resurrect(HealthData.MaxHealth);
        }

        /// <summary>
        /// Starts the deathTimer and despawns this object.
        /// </summary>
        public virtual void StartDeathTimer()
        {
            poolSystem.StartDeathTimer(DeathTime, ResourceItem.Name, transform.parent);
            poolSystem.DespawnObject(ResourceItem.Name, gameObject);
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
            InventorySystem.AddItem(ResourceItem, AmountToGive);

            ServerInteract();
        }

        /// <summary>
        /// Plays interacting sound.
        /// </summary>
        public virtual void PlayInteractingSound()
        {

        }

        /// <summary>
        /// Let the player interact to server resource.
        /// </summary>
        [Server]
        public virtual void ServerInteract()
        {
            HealthData.TakeDamage(DamageAmount);
            ReceiveInteract();

        }

        /// <summary>
        /// Calls all functionalities that need to happen when someone else is interacting with this Resource
        /// </summary>
        [ObserversRpc]
        public virtual void ReceiveInteract()
        {
            //playanimation
        }
    }
}
