using System.Collections.Generic;
using GameEventSystem;
using UnityEditor;
using UnityEngine;

namespace hex {
    // ReSharper disable RedundantDefaultMemberInitializer

    public class Grid : MonoBehaviour {
        private Dictionary<HexCoordinates, HexCell> _grid;

        [SerializeField] private GameObject cellsGameObject = default;
        [SerializeField] private int width = 6;

        [SerializeField] private int height = 6;

        [SerializeField] private HexCell cellPrefab = default;

        [SerializeField] private GameEvent clickedCell = default;
        private Camera mainCamera;

        private void Awake() {
            mainCamera = Camera.main;

            _grid = new Dictionary<HexCoordinates, HexCell>();
            for (int z = 0; z < height; z++) {
                for (var x = 0; x < width; x++) {
                    HexCoordinates c = HexCoordinates.FromOffsetCoordinates(x, z);
                    createFixedCell(c);
                }
            }
        }

        private void createFixedCell(HexCoordinates c) {
            var cell = Instantiate(cellPrefab, cellsGameObject.transform, false);
            cell.transform.localPosition = c.ToPosition();
            cell.coordinates = c;
            _grid[cell.coordinates] = cell;
        }

        private void OnDrawGizmos() {
            if (_grid == null) return;
            foreach (var hexCoordinates in _grid.Keys) {
                var position = hexCoordinates.ToPosition();
                Handles.Label(position, hexCoordinates + ": " + position);
            }
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
            var cell = this[coordinates];
            if (cell && !(cell.IsHovered)) {
                cell.setHighlighted();
            }

            return cell;
        }

        public HexCell this[HexCoordinates c] {
            get {
                if (_grid.TryGetValue(c, out HexCell value)) {
                    return value;
                }

                return null;
            }
            set { _grid[c] = value; }
        }

        public bool CellAvailable(HexCoordinates c) {
            if (_grid.TryGetValue(c, out HexCell value)) {
                return value.available();
            }

            return false;
        }

        public void MoveTopping(HexCoordinates from, HexCoordinates to) {
            HexCell fromCell = this[from];
            HexCell toCell = this[to];
            if (toCell != null) {
                toCell.moveToppingTo(fromCell);
            }
        }
    }
}