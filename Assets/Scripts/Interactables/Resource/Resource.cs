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
        /// Sets PoolSystem and HealthData Variables and add StartDeathTimer to the onDie Event.
        /// </summary>
        public virtual void Start()
        {
            poolSystem = PoolSystem.Instance;
            HealthData = GetComponent<HealthData>();
            HealthData.onDie.AddListener(StartDeathTimer);
            HasHealthData = true;
        }

        /// <summary>
        /// Resurrect the player if it has healthData.
        /// </summary>
        public virtual void OnEnable()
        {
            if(HasHealthData)
                HealthData.Resurrect(HealthData.MaxHealth);
        }

        /// <summary>
        /// Calls on every functionality, that needs to happen when interacting with the resources. 
        /// </summary>
        [ServerRpc]
        public virtual void Interact()
        {
            PlayInteractingSound();
            InventorySystem.AddItem(ResourceItem, AmountToGive);

            ServerInteract();
        }

        [Server]
        public void ServerInteract()
        {
            HealthData.TakeDamage(DamageAmount);
            ReceiveInteract();
        }

        [ObserversRpc]
        public void ReceiveInteract()
        {
            //playanimation
        }

        /// <summary>
        /// Plays interacting sound.
        /// </summary>
        public virtual void PlayInteractingSound()
        {

        }

        /// <summary>
        /// Starts the deathTimer and despawns this object.
        /// </summary>
        public virtual void StartDeathTimer()
        {
            poolSystem.StartDeathTimer(DeathTime, ResourceItem.Name, transform.parent);
            poolSystem.DespawnObject(ResourceItem.Name, gameObject);
        }
    }
}
