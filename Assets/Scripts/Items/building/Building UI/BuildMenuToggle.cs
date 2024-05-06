using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Examen.Building.BuildingUI
{
    [RequireComponent(typeof(Button))]
    public class BuildMenuToggle : MonoBehaviour
    {
        [SerializeField] private BuildMenu _buildMenu;
        [SerializeField] private GameObject _hotBar;

        private Button _toggle;

        private void Awake()
        {
            _toggle = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _toggle.onClick.AddListener(ToggleMenu);
        }

        private void OnDisable()
        {
            _toggle.onClick.RemoveListener(ToggleMenu);
        }

        private void ToggleMenu()
        {
            _buildMenu.gameObject.SetActive(!_buildMenu.gameObject.activeSelf);
            _hotBar.SetActive(!_hotBar.activeSelf);
        }
    }
}