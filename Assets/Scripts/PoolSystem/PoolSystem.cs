using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolSystem : MonoBehaviour
{
    private Dictionary<string, List<GameObject>> _objectsQueu = new();
    private Dictionary<string, List<GameObject>> _objectsActive = new();

    private void Start() => DontDestroyOnLoad(gameObject);

    public void AddActiveObjects(string nameTag, List<GameObject> gameObjects)
    {
        if (_objectsActive.ContainsKey(nameTag))
            _objectsActive[nameTag].AddRange(gameObjects);
        else
            _objectsActive.Add(nameTag, gameObjects);
    }

    public void SpawnObject(string nameTag, GameObject spawnPrefab, Transform parentTransform = null) => SpawnObject(nameTag, parentTransform, spawnPrefab);

    public void SpawnObject(string nameTag, Transform parentTransform = null, GameObject spawnPrefab = null)
    {
        if(_objectsQueu.ContainsKey(nameTag) && _objectsQueu[nameTag].Count > 0)
        {
            List<GameObject> objects = _objectsQueu[nameTag];

            MoveObjectToActiveScene(objects[objects.Count], parentTransform);
            AddActiveObject(nameTag, objects[objects.Count]);

            objects[objects.Count].SetActive(true);
            objects.RemoveAt(objects.Count);
            return;
        }

        if (spawnPrefab == null) return;
        
        GameObject newGameObject = Instantiate(spawnPrefab);
        MoveObjectToActiveScene(newGameObject, parentTransform);
        AddActiveObject(nameTag, newGameObject);
    }

    public void DespawnObject(string nameTag)
    {
        if (_objectsActive.ContainsKey(nameTag) && _objectsActive[nameTag].Count > 0)
        {
            List<GameObject> objects = _objectsActive[nameTag];

            MoveObjectToDontDestroyOnLoadScene(objects[objects.Count]);
            AddQueuObject(nameTag, objects[objects.Count]);

            objects[objects.Count].SetActive(false);
            objects.RemoveAt(objects.Count);
            return;
        }

        Debug.LogError("Can't find object you want to despawn");
    }

    private void MoveObjectToActiveScene(GameObject movingObject,Transform parentTransform)
    {
        if (parentTransform == null)
            SceneManager.MoveGameObjectToScene(movingObject, SceneManager.GetActiveScene());
        else
            movingObject.transform.parent = parentTransform;
    }

    private void MoveObjectToDontDestroyOnLoadScene(GameObject movingObject) => movingObject.transform.parent = transform;

    private void AddActiveObject(string nameTag, GameObject gameObject)
    {
        if (_objectsActive.ContainsKey(nameTag))
            _objectsActive[nameTag].Add(gameObject);
        else
            _objectsActive.Add(nameTag,new List<GameObject>() { gameObject });
    }

    private void AddQueuObject(string nameTag, GameObject gameObject)
    {
        if (_objectsQueu.ContainsKey(nameTag))
            _objectsQueu[nameTag].Add(gameObject);
        else
            _objectsQueu.Add(nameTag, new List<GameObject>() { gameObject });
    }
}

//In inspector set which objects are created by default
//Spawns in the objects
//Saves spawned in gameobjects in a list
// when object despawns it stores it in a list with the rest of object
// when respawning, it get it from the list. if there aren't any it will create a new 1 