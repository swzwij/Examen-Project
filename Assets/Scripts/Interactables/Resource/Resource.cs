using Examen.Inventory;
using Examen.Items;
using Examen.Poolsystem;
using MarkUlrich.Health;
using System;
using System.Collections;
using UnityEngine;

namespace Examen.Interactables.Resource
{
    [RequireComponent(typeof(HealthData))]
    public class Resource : MonoBehaviour, Interactable
    {
        [HideInInspector] public HealthData HealthData;
        [HideInInspector] public PoolSystem poolSystem;

        public Item ResourceItem;
        public int AmountToGive = 1;
        public int DeathTime;

        public const int DamageAmount = 1;
        public Action OnRespawn;

        public virtual void Interact()
        {
            Debug.Log("interact");
            PlayInteractingSound();
            //playanimation
            HealthData.TakeDamage(DamageAmount);
            InventorySystem.AddItem(ResourceItem, AmountToGive);
        }

        public virtual void PlayInteractingSound()
        {

        }

        public virtual void Start()
        {
            poolSystem = PoolSystem.Instance;
            HealthData = GetComponent<HealthData>();
            HealthData.onDie.AddListener(StartDeathTimer);
        }

        public virtual void StartDeathTimer()
        {
            poolSystem.DespawnObject(ResourceItem.Name);
            StartCoroutine(DeathTimer());
        }

        private IEnumerator DeathTimer()
        {
            yield return new WaitForSeconds(DeathTime);

            poolSystem.SpawnObject(ResourceItem.Name);
            OnRespawn?.Invoke();
        }
    }
}
