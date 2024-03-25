using Examen.Inventory;
using UnityEngine;

namespace Examen.Items
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item", order = 1)]
    public class Item : ScriptableObject
    {
        public string Name;
    }
}
