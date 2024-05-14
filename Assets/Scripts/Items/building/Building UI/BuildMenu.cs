using Examen.Inventory;
using UnityEngine;

namespace Examen.Building.BuildingUI
{
    public class BuildMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _inventory;

        public void Toggle(bool isActive)
        {
            _inventory.SetActive(!isActive);
            gameObject.SetActive(isActive);
        }
    }
}