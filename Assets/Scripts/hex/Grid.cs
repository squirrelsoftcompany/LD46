using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace hex {
    public class Grid : MonoBehaviour {
        private HexCell[] cells;
        [SerializeField] private GameObject cellsGameObject;
        [SerializeField] private int width = 6;

        [SerializeField] private int height = 6;

        [SerializeField] private HexCell cellPrefab;

        private Camera mainCamera;

        private void Awake() {
            mainCamera = Camera.main;

            cells = new HexCell[height * width];
            for (int z = 0, i = 0; z < height; z++) {
                for (var x = 0; x < width; x++) {
                    createFixedCell(x, z, i++);
                }
            }
        }

        private void createCell(int x, int z, int i) {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * HexMetrics.innerRadius * 2f;
            position.y = 0f;
            position.z = z * HexMetrics.outerRadius * 1.5f;
            var cell = cells[i] = Instantiate(cellPrefab);
            Transform cellTransform;
            (cellTransform = cell.transform).SetParent(cellsGameObject.transform, false);
            cellTransform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        }

        private void createFixedCell(int x, int z, int i) {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * 3.4f;
            position.y = 0f;
            // odd
            position.z = z * 1.5f * 2f;
            var cell = cells[i] = Instantiate(cellPrefab, cellsGameObject.transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
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
            Debug.Log("hover : " + coordinates);
            var index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            var cell = cells[index];
            if (!cell.IsHovered) {
                // cell.hover.SetActive(true);
                // cell.normal.SetActive(false);
                cell.setHighlighted();
            }
        }
    }
}