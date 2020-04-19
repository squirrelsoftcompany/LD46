using System.Collections.Generic;
using UnityEngine;

namespace hex {
    public class Grid : MonoBehaviour {
        private Dictionary<HexCoordinates, HexCell> _grid;

        [SerializeField] private GameObject cellsGameObject;
        [SerializeField] private int width = 6;

        [SerializeField] private int height = 6;

        [SerializeField] private HexCell cellPrefab;

        private Camera mainCamera;

        private void Awake() {
            mainCamera = Camera.main;

            _grid = new Dictionary<HexCoordinates, HexCell>();
            for (int z = 0, i = 0; z < height; z++) {
                for (var x = 0; x < width; x++) {
                    createFixedCell(x, z);
                }
            }
        }

        private void createCell(int x, int z) {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerRadius * 2f;
            position.y = 0f;
            position.z = z * HexMetrics.outerRadius * 1.5f;
            var cell = Instantiate(cellPrefab);
            Transform cellTransform;
            (cellTransform = cell.transform).SetParent(cellsGameObject.transform, false);
            cellTransform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            _grid[cell.coordinates] = cell;
        }

        private void createFixedCell(int x, int z) {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerRadius * 2f;
            position.y = 0f;
            position.z = z * 1.5f * HexMetrics.outerRadius;
            var cell = Instantiate(cellPrefab, cellsGameObject.transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            _grid[cell.coordinates] = cell;
        }

        private void Update() {
            var inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(inputRay, out var hit)) {
                hoverCell(hit.point);
            }
        }

        private void hoverCell(Vector3 position) {
            position = transform.InverseTransformPoint(position);
            var coordinates = HexCoordinates.FromPosition(position);
            // Debug.Log("hover : " + coordinates);
            var cell = this[coordinates];
            if (cell && !(cell.IsHovered)) {
                cell.setHighlighted();
            }
        }

        public HexCell this[HexCoordinates c]
        {
            get
            {
                if (_grid.TryGetValue(c, out HexCell value))
                {
                    return value;
                }
                return null;
            }
            set { _grid[c] = value; }
        }

        public bool CellAvaialable(HexCoordinates c)
        {
            if (_grid.TryGetValue(c, out HexCell value))
            {
                return value.available();
            }
            return false;
        }

        public void MoveTopping(HexCoordinates from, HexCoordinates to)
        {
            HexCell fromCell = this[from];
            HexCell toCell = this[to];
            if (toCell != null)
            {
                toCell.moveToppingTo(fromCell);
            }
        }
    }
}