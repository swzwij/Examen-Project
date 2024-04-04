using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Examen.Building
{
    static class StructureList
    {
        private static HashSet<GameObject> _allStructures = new();

        public static HashSet<GameObject> GetList() => _allStructures;

        public static void AddStructure(GameObject structure) => _allStructures.Add(structure);

        public static void RemoveStructure(GameObject structure) => _allStructures.Remove(structure);

        public static bool CheckHasStructure(GameObject structure) => _allStructures.Contains(structure);
    }
}

