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

        public void Initialize(ItemInstance item, int amount)
        {
            _image.sprite = item.Name == "Wood" ? _sprites[0] : _sprites[1];
            _text.text = $"{amount}";
        }

        public void UpdateItem(int amount)
        {
            _text.text = $"{amount}";
        }
    }
}