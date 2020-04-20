using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private List<Building> _lakes = default;
        [SerializeField] private List<Ground> _grounds = default;
        [SerializeField] private List<Props> _props = default;

        [SerializeField] private NavMeshSurface navMeshSurface = default;

        [SerializeField] private GameEvent clickedCell = default;

        // [SerializeField]private GameObject planeCollisions = default;
        private Camera mainCamera;

        public Grid myGrid => _grid;

        private void Awake() {
            mainCamera = Camera.main;

            //_grid = new Grid(width, height, cellPrefab.gameObject, cellsGameObject.transform);
            _grid = new Grid();
            LevelManager.Instance.GetLevelData(out _buildings, out _lakes, out _grounds, out _props, out width, out height, out cellPrefab);
            var generator = new GridGenerator(myGrid, _buildings, _lakes, _grounds, _props, width, height, cellPrefab,
                cellsGameObject.transform);
            generator.Generate();
            // navMeshSurface.BuildNavMesh();
            // planeCollisions.transform.localScale = new Vector3(width * HexMetrics.innerRadius, 1, 
            //     height*HexMetrics.outerRadius);
        }

        private void Start() {
            navMeshSurface.BuildNavMesh();
        }

        // private void OnDrawGizmos() {
        //     if (_grid == null) return;
        //     foreach (var hexCoordinates in _grid.Keys) {
        //         var position = hexCoordinates.ToPosition();
        //         Handles.Label(position, hexCoordinates + ": " + position);
        //     }
        // }

        private void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(HexCoordinates.FromOffsetCoordinates(0, 0).ToPosition(),
                HexCoordinates.FromOffsetCoordinates(0, height - 1).ToPosition());
            Gizmos.DrawLine(HexCoordinates.FromOffsetCoordinates(0, 0).ToPosition(),
                HexCoordinates.FromOffsetCoordinates(width - 1, 0).ToPosition());
            Gizmos.DrawLine(HexCoordinates.FromOffsetCoordinates(width - 1, height - 1).ToPosition(),
                HexCoordinates.FromOffsetCoordinates(0, height - 1).ToPosition());
            Gizmos.DrawLine(HexCoordinates.FromOffsetCoordinates(width - 1, height - 1).ToPosition(),
                HexCoordinates.FromOffsetCoordinates(width - 1, 0).ToPosition());
        }

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
            var cell = myGrid[coordinates];
            if (cell && cell.Highlight != Highlight.CURRENT_ACTION) {
                cell.Highlight = cell.available() ? Highlight.HIGHLIGHTED : Highlight.INVALID;
            }

            foreach (var hexCoordinates in _grid.Keys.Where(hexCoordinates =>
                !coordinates.Equals(hexCoordinates)
                && (_grid[hexCoordinates].Highlight == Highlight.HIGHLIGHTED ||
                    _grid[hexCoordinates].Highlight == Highlight.INVALID))) {
                _grid[hexCoordinates].Highlight = Highlight.NORMAL;
            }

            return cell;
        }
    }
}