using System.Collections.Generic;
using GameEventSystem;
using GridGeneration;
using UnityEngine;
using UnityEngine.AI;

namespace hex {
    // ReSharper disable RedundantDefaultMemberInitializer

    public class GridManager : MonoBehaviour {
        private Grid _grid;

        [SerializeField] private GameObject cellsGameObject = default;
        [SerializeField] private HexCell cellPrefab = default;
        [SerializeField] private int width = 6;
        [SerializeField] private int height = 6;

        [SerializeField] private List<Building> _buildings = default;

        [SerializeField] private NavMeshSurface navMeshSurface = default;
 
        [SerializeField] private GameEvent clickedCell = default;
        private Camera mainCamera;

        private void Awake() {
            mainCamera = Camera.main;

            _grid = new Grid(width, height, cellPrefab.gameObject, cellsGameObject.transform);

            var generator = new GridGenerator(_grid, _buildings);
            generator.Generate();
            navMeshSurface.BuildNavMesh();
        }

        // private void OnDrawGizmos() {
        //     if (_grid == null) return;
        //     foreach (var hexCoordinates in _grid.Keys) {
        //         var position = hexCoordinates.ToPosition();
        //         Handles.Label(position, hexCoordinates + ": " + position);
        //     }
        // }

        private void Update() {
            var inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(inputRay, out var hit)) {
                var hoveredCell = hoverCell(hit.point);
                if (Input.GetMouseButton(0) && hoveredCell) {
                    // Clicked on that cell
                    manageClickedCell(hoveredCell);
                }
            }
        }

        private void manageClickedCell(HexCell cell) {
            clickedCell.sentMonoBehaviour = cell;
            clickedCell.Raise();
        }

        private HexCell hoverCell(Vector3 position) {
            position = transform.InverseTransformPoint(position);
            var coordinates = HexCoordinates.FromPosition(position);
            // Debug.Log("hover : " + coordinates);
            var cell = _grid[coordinates];
            if (cell && !(cell.IsHovered)) {
                cell.setHighlighted();
            }

            return cell;
        }
    }
}