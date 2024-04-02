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
        }

        /// <summary>
        /// Resurrect the player if it has healthData.
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

            HealthData.Resurrect(HealthData.MaxHealth);

            SetNewPostion(transform.position);
        }

        /// <summary>
        /// Calls on every functionality, that needs to happen when interacting with the resources. 
        /// </summary>
        public virtual void Interact()
        {
            PlayInteractingSound();
            InventorySystem.AddItem(ResourceItem, AmountToGive);

            ServerInteract();
        }

        [Server]
        public virtual void ServerInteract()
        {
            HealthData.TakeDamage(DamageAmount);
            ReceiveInteract();
        }

        [ObserversRpc]
        public virtual void ReceiveInteract()
        {
            //playanimation
        }

        [ObserversRpc]
        public virtual void SetNewPostion(Vector3 newPosition)
        {
            transform.position = newPosition;
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

        [ObserversRpc]
        public void ToggleGameobject() => gameObject.SetActive(gameObject.activeSelf);

    }
}
