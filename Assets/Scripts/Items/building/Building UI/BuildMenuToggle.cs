using UnityEngine;
using UnityEngine.UI;

namespace Examen.Building.BuildingUI
{
    [RequireComponent(typeof(Button))]
    public class BuildMenuToggle : MonoBehaviour
    {
        [SerializeField] private BuildMenu _buildMenu;
        [SerializeField] private GameObject _hotBar;

        /// <summary>
        /// Toggles the build menu.
        /// </summary>
        /// <param name="isActive">Whether the build menu should activate</param>
        public void ToggleBuildMenu(bool isActive)
        {
            _buildMenu.gameObject.SetActive(isActive);
            _hotBar.SetActive(!isActive);
        }
    }
}