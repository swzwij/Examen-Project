using Examen.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Examen.Inventory
{
    public class InventoryDisplayItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Text _text;

        [SerializeField] private Sprite[] _sprites;

        /// <summary>
        /// Initializes the display item with the item and amount.
        /// </summary>
        /// <param name="item">The name of the item.</param>
        /// <param name="amount">The amount of the item.</param>
        public void Initialize(string item, int amount)
        {
            _image.sprite = item == "Stone" ? _sprites[0] : _sprites[1];
            _text.text = $"{amount}";
        }

        /// <summary>
        /// Updates the item with the new amount.
        /// </summary>
        /// <param name="amount">The new amount of the item.</param>
        public void UpdateItem(int amount) => _text.text = $"{amount}";
    }
}