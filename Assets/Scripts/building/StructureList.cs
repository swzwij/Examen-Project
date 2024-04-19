using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Examen.Building
{
    static class StructureList
    {
        private static readonly HashSet<Examen.Structure.BaseStructure> _allStructures = new();

        public static HashSet<Examen.Structure.BaseStructure> GetList() => _allStructures;

        public static void AddStructure(Examen.Structure.BaseStructure structure) => _allStructures.Add(structure);

        public static void RemoveStructure(Examen.Structure.BaseStructure structure) => _allStructures.Remove(structure);

        public static bool CheckHasStructure(Examen.Structure.BaseStructure structure) => _allStructures.Contains(structure);
    }
}

