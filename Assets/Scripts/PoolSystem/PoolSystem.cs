using MarkUlrich.Utils;
using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Examen.Poolsystem
{
    public class PoolSystem : SingletonInstance<PoolSystem>
    {
        private Dictionary<string, List<GameObject>> _objectsQueu = new();
        private Dictionary<string, List<GameObject>> _objectsActive = new();

        public void AddActiveObject(string nameTag, GameObject gameObject)
        {
            if (_objectsActive.ContainsKey(nameTag))
                _objectsActive[nameTag].Add(gameObject);
            else
                _objectsActive.Add(nameTag, new List<GameObject>() { gameObject });
        }

        public void SpawnObject(string nameTag, GameObject spawnPrefab, Transform parentTransform = null) => SpawnObject(nameTag, parentTransform, spawnPrefab);

        public void SpawnObject(string nameTag, Transform parentTransform = null, GameObject spawnPrefab = null)
        {
            if (_objectsQueu.ContainsKey(nameTag) && _objectsQueu[nameTag].Count > 0)
            {
                List<GameObject> objects = _objectsQueu[nameTag];
                int objectCount = objects.Count - 1;

                MoveObjectToActiveScene(objects[objectCount], parentTransform);
                AddActiveObject(nameTag, objects[objectCount]);

                objects[objectCount].SetActive(true);
                objects.RemoveAt(objectCount);
                return;
            }

            if (spawnPrefab == null) return;

            GameObject newGameObject = Instantiate(spawnPrefab);
            MoveObjectToActiveScene(newGameObject, parentTransform);
            AddActiveObject(nameTag, newGameObject);
        }

        public void DespawnObject(string nameTag, GameObject despawnObject)
        {
            if (_objectsActive.ContainsKey(nameTag) && _objectsActive[nameTag].Contains(despawnObject))
            {
                List<GameObject> objects = _objectsActive[nameTag];

                MoveObjectToDontDestroyOnLoadScene(despawnObject);
                AddQueuObject(nameTag, despawnObject);

                despawnObject.SetActive(false);
                objects.Remove(despawnObject);
                return;
            }

            Debug.LogError($"Can't find object {nameTag} to despawn");
        }

        public void StartDeathTimer(int amountOfTimer, string resourceName, Transform previousParentTransform) => StartCoroutine(DeathTimer(amountOfTimer, resourceName,previousParentTransform));

        private IEnumerator DeathTimer(int amountOfTimer, string resourceName, Transform previousParentTransform)
        {
            yield return new WaitForSeconds(amountOfTimer);

            SpawnObject(resourceName, previousParentTransform);
        }

        private void MoveObjectToActiveScene(GameObject movingObject, Transform parentTransform)
        {
            if (parentTransform == null)
                SceneManager.MoveGameObjectToScene(movingObject, SceneManager.GetActiveScene());
            else
                movingObject.transform.parent = parentTransform;
        }

        private void MoveObjectToDontDestroyOnLoadScene(GameObject movingObject) => movingObject.transform.parent = transform;

        private void AddQueuObject(string nameTag, GameObject gameObject)
        {
            if (_objectsQueu.ContainsKey(nameTag))
                _objectsQueu[nameTag].Add(gameObject);
            else
                _objectsQueu.Add(nameTag, new List<GameObject>() { gameObject });
        }
    }
}