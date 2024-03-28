using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public void SpawnResources(Dictionary<GameObject, float> resourcesToSpawn)
    {
        foreach (var resource in resourcesToSpawn)
        {
            Debug.Log($"Spawn {resource.Value} {resource.Key.name}");

            for (int i = 0; i < resource.Value; i++)
            {
                Instantiate(resource.Key); 
            }
        }
    }
}
