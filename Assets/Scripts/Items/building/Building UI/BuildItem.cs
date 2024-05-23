using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Examen.Building.BuildingUI
{
    public class BuildItem : MonoBehaviour
    {
        [SerializeField] private int _levelRequirement;

        public int LevelRequirement => _levelRequirement;
    }
}