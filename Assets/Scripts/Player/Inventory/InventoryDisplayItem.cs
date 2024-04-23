using Examen.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Examen.Inventory
{
    public class InventoryDisplayItem : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private Text _text;

        public void Initialize(Item item, int amount)
        {
            _image.sprite = item.Icon;
            _text.text = $"{amount}";
        }

        public void UpdateItem(int amount)
        {
            _text.text = $"{amount}";
        }
    }
}