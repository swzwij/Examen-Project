using MarkUlrich.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Examen.Poolsystem
{
    public class PoolSystem : SingletonInstance<PoolSystem>
    {
        private Dictionary<string, List<GameObject>> _objectStack = new();
        private Dictionary<string, List<GameObject>> _objectsActive = new();

        /// <summary>
        /// Puts the given gameobject in the active objects dictonary under the given tag.
        /// </summary>
        /// <param name="nameTag">The name of the catergory, you want the gameobject to be in.</param>
        /// <param name="gameObject">The object you want to add to the active objects dictionary.</param>
        public void AddActiveObject(string nameTag, GameObject gameObject)
        {
            if (_objectsActive.ContainsKey(nameTag))
                _objectsActive[nameTag].Add(gameObject);
            else
                _objectsActive.Add(nameTag, new List<GameObject>() { gameObject });
        }

        /// <summary>
        /// Creates new object if there aren't objects in the pool or put objects from the pool into the scene.
        /// </summary>
        /// <param name="nameTag">The name of the category the object is in.</param>
        /// <param name="spawnPrefab">The object you want to spawn in.</param>
        /// <param name="parentTransform">The object you want the spawnobject to be the childobject of.</param>
        public GameObject SpawnObject(string nameTag, GameObject spawnPrefab, Transform parentTransform = null) 
            => SpawnObject(nameTag, parentTransform, spawnPrefab);

        /// <summary>
        /// Creates new object if there aren't objects in the pool or put objects from the pool into the scene.
        /// </summary>
        /// <param name="nameTag">The name of the category the object is in.</param>
        /// <param name="parentTransform">The object you want the spawnobject to be the childobject of.</param>
        /// <param name="spawnPrefab">The object you want to spawn in.</param>
        public GameObject SpawnObject(string nameTag, Transform parentTransform = null, GameObject spawnPrefab = null)
        {
            if (_objectStack?.ContainsKey(nameTag) == true && _objectStack[nameTag].Any())
            {
                GameObject lastObject = _objectStack[nameTag].LastOrDefault();
                _objectStack[nameTag].Remove(lastObject);

                MoveObjectToActiveScene(lastObject, parentTransform);
                AddActiveObject(nameTag, lastObject);

                lastObject.SetActive(true);
                return lastObject;
            }

            if (spawnPrefab == null) 
                return null;

            GameObject newGameObject = Instantiate(spawnPrefab);
            newGameObject.name = nameTag;
            MoveObjectToActiveScene(newGameObject, parentTransform);
            AddActiveObject(nameTag, newGameObject);

            return newGameObject;
        }

        /// <summary>
        /// Puts the given object in the poolsystem.
        /// </summary>
        /// <param name="nameTag">The name of the category the object should be in.</param>
        /// <param name="despawnObject">The object you want to despawn.</param>
        public void DespawnObject(string nameTag, GameObject despawnObject)
        {
            if (!(_objectsActive.ContainsKey(nameTag) && _objectsActive[nameTag].Contains(despawnObject)))
            {
                Debug.LogError($"Can't find object {nameTag} to despawn");
                return;
            }

            MoveObjectToDontDestroyOnLoadScene(despawnObject);
            AddQueuObject(nameTag, despawnObject);

            despawnObject.SetActive(false);
            _objectsActive[nameTag].Remove(despawnObject);
        }

        /// <summary>
        /// Starts coroutine DeathTimer.
        /// </summary>
        /// <param name="respawnTime">How long do you want the death timer to last.</param>
        /// <param name="objectName">Name of the object you want to bring back after the timer.</param>
        /// <param name="previousParentTransform">The parent object you want the object to appear under.</param>
        public void StartRespawnTimer(int respawnTime, string objectName, Transform previousParentTransform) 
            => StartCoroutine(RespawnTimer(respawnTime, objectName, previousParentTransform));

        private IEnumerator RespawnTimer(int respawnTime, string objectName, Transform previousParentTransform)
        {
            yield return new WaitForSeconds(respawnTime);

            SpawnObject(objectName, previousParentTransform);
        }

        private void MoveObjectToActiveScene(GameObject movingObject, Transform parentTransform)
        {
/*            if (parentTransform == null)
            {
                movingObject.transform.parent = 
                //SceneManager.MoveGameObjectToScene(movingObject, SceneManager.GetActiveScene());
            }
            else*/
                movingObject.transform.parent = parentTransform;
        }

        private void MoveObjectToDontDestroyOnLoadScene(GameObject movingObject) 
            => movingObject.transform.parent = transform;

        private void AddQueuObject(string nameTag, GameObject gameObject)
        {
            if (_objectStack.ContainsKey(nameTag))
                _objectStack[nameTag].Add(gameObject);
            else
                _objectStack.Add(nameTag, new List<GameObject>() { gameObject });
        }
    }
}