using Examen.Inventory;
using Examen.Items;
using MarkUlrich.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealthData))]
public class Stone : MonoBehaviour, Interactable
{
    [SerializeField] private Item _stoneItem;

    private HealthData _healthData;

    public void Interact()
    {
        Debug.Log("interact");
        PlayInteractingSound();
        //playanimation
        _healthData.TakeDamage(1);
        InventorySystem.AddItem(_stoneItem, 1);
    }

    public void PlayInteractingSound()
    {

    }

    private void Start()
    {
        _healthData = GetComponent<HealthData>();
        //_healthData.onDie.AddListener();
    }
}
