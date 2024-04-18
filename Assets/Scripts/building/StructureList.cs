using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Examen.Building
{
    static class StructureList
    {
        private static readonly HashSet<BaseStructure> _allStructures = new();

        public static HashSet<BaseStructure> GetList() => _allStructures;

        public static void AddStructure(BaseStructure structure) => _allStructures.Add(structure);

        public static void RemoveStructure(BaseStructure structure) => _allStructures.Remove(structure);

        public static bool CheckHasStructure(BaseStructure structure) => _allStructures.Contains(structure);
    }
}

