using Examen.Items;
using System;
using UnityEngine;

namespace Examen.Structures
{
    [Serializable]
    public struct StructureCost
    {
        [SerializeField] private Item Item;

        public int Amount;
        public readonly string ItemName => Item.Name;
    }
}