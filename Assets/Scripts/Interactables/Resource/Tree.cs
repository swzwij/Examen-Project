using Examen.Inventory;
using Examen.Items;
using MarkUlrich.Health;
using UnityEngine;

[RequireComponent(typeof(HealthData))]
public class Tree : MonoBehaviour, Interactable
{
    [SerializeField] private Item woodItem;

    private HealthData _healthData;

    public void Interact()
    {
        Debug.Log("interact");
        PlayInteractingSound();
        //playanimation
        _healthData.TakeDamage(1);
        InventorySystem.AddItem(woodItem, 1);
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
