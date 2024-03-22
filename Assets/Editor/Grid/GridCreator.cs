using Examen.Pathfinding.Grid;
using UnityEditor;
using UnityEngine;

namespace Examen.Editors.Grid
{
    [CustomEditor(typeof(GridSystem))]
    public class GridCreator : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GridSystem grid = (GridSystem)target;

            if (GUILayout.Button("Create Grid"))
                grid.CreateGrid();

            if (GUILayout.Button("Update Cell 1,1"))
                grid.UpdateCell(1, 1);

            if (GUILayout.Button("Clear Grid"))
                grid.ClearGrid();
        }
    }
}
