using Examen.Inventory;
using Examen.Player.PlayerDataManagement;
using UnityEngine;

namespace Examen.Building.BuildingUI
{
    public class BuildMenu : MonoBehaviour
    {
        [SerializeField] private BuildItem[] _buildItems;
        [SerializeField] private GameObject _inventory;
        [SerializeField] private PlayerDataHandler _playerDataHandler;

        private void Awake()
        {
            foreach (BuildItem buildItem in _buildItems)
                buildItem.gameObject.SetActive(false);

            UpdateBuildMenu(_playerDataHandler.Exp);
        }

        private void OnEnable() => PlayerDatabase.Instance.OnLevelChanged += UpdateBuildMenu;

        private void OnDisable() => PlayerDatabase.Instance.OnLevelChanged -= UpdateBuildMenu;

        /// <summary>
        /// Toggles the build menu.
        /// </summary>
        /// <param name="isActive">Whether the build menu should activate.</param>
        public void Toggle(bool isActive)
        {
            _inventory.SetActive(!isActive);
            gameObject.SetActive(isActive);
        }

        private void UpdateBuildMenu(int level)
        {
            foreach (BuildItem buildItem in _buildItems)
            {
                if (level >= buildItem.LevelRequirement)
                    buildItem.gameObject.SetActive(true);
                else
                    buildItem.gameObject.SetActive(false);
            }
        }
    }
}