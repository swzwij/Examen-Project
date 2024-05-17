using Examen.Inventory;
using UnityEngine;

namespace Examen.Building.BuildingUI
{
    public class BuildMenu : MonoBehaviour
    {
        [SerializeField] private GameObject _inventory;

        /// <summary>
        /// Toggles the build menu.
        /// </summary>
        /// <param name="isActive">Whether the build menu should activate.</param>
        public void Toggle(bool isActive)
        {
            _inventory.SetActive(!isActive);
            gameObject.SetActive(isActive);
        }
    }
}