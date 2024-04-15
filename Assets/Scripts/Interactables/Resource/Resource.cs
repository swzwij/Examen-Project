using Examen.Inventory;
using Examen.Items;
using Examen.Networking;
using Examen.Pathfinding.Grid;
using Examen.Poolsystem;
using FishNet.Connection;
using FishNet.Object;
using MarkUlrich.Health;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Android;

namespace Examen.Interactables.Resource
{
    [RequireComponent(typeof(HealthData))]
    public class Resource : Interactable
    {
        [SerializeField] protected Item p_resourceItem;
        [SerializeField] protected int p_supplyAmount = 1;
        [SerializeField] protected int p_respawnTime;

        protected HealthData p_healthData;

        public Item ResourceItem => p_resourceItem;

        private void OnEnable() => RespawnResource();

        private void Start() => ServerInstance.Instance.OnServerStarted += InitResource; 

        /// <summary>
        /// If object isServer, it resurrects this gameobject and calls for the clients to mimic its position and active state.
        /// </summary>
        public virtual void InitResource()
        {
            if (!IsServer)
                return;

            p_healthData = GetComponent<HealthData>();

            p_healthData.onDie.AddListener(StartRespawnTimer);
            p_healthData.onDie.AddListener(DisableObject);

            p_healthData.onResurrected.AddListener(EnableObject);
        }

        [Server]
        protected virtual void RespawnResource()
        {
            SetRandomPosition();
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
        public void EnableObject() => gameObject.SetActive(true);

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

        public virtual void SetRandomPosition()
        {
            Cell currentCell = GridSystem.Instance.GetCellFromWorldPosition(transform.TransformPoint(transform.position));
            Debug.Log(currentCell.ToString());
            transform.position = RandomisePosition(currentCell.Nodes);

            SetNewPostion(transform.position);

            StartCoroutine(WaitToUpdateCell(currentCell));
        }
        
        IEnumerator WaitToUpdateCell(Cell currentCell)
        {
            yield return new WaitForSeconds(0.1f);
            GridSystem.Instance.UpdateCell(currentCell.CellX, currentCell.CellY);
        }

        public Vector3 RandomisePosition(HashSet<Node> nodes)
        {
            int randomNumber = Random.Range(0, nodes.Count);
            Node randomNode = nodes.ElementAt(randomNumber);

            if(!randomNode.IsWalkable)
                return RandomisePosition(nodes);

            //Debug.Log(randomNode.Position);

            return randomNode.Position;
        }

        /// <summary>
        /// Calls all functionalities that need to happen when you are interacting with this Resource
        /// and sends that to the server.
        /// </summary>
        [Server]
        public override void Interact(NetworkConnection connection, float damageAmount = 0)
        {
            PlayInteractingSound();

            ServerInventory.Instance.AddItem(connection, p_resourceItem, p_supplyAmount);
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

        private void OnDestroy() => ServerInstance.Instance.OnServerStarted -= InitResource;
    }
}
