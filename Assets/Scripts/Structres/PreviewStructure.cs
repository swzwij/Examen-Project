using System.Collections.Generic;
using UnityEngine;

namespace Examen.Structure
{
    public class PreviewStructure : MonoBehaviour
    {
        [SerializeField] private List<MeshRenderer> _meshRenderers;

        public List<MeshRenderer> MeshRenderers => _meshRenderers;
    }
}