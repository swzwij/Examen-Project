using UnityEngine;

namespace Examen.Items
{
    [CreateAssetMenu(fileName = "Item", menuName = "Items/New Item", order = 1)]
    public class Item : ScriptableObject
    {
        [SerializeField] private string _name;

        public string Name => _name;
    }
}
