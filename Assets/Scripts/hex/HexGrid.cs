using TMPro;
using UnityEngine;

namespace hex {
    public class HexGrid : MonoBehaviour {
        [SerializeField] private int width = 6;

        [SerializeField] private int height = 6;

        [SerializeField] private HexCell cellPrefab;
        [SerializeField] private TMP_Text cellLabelPrefab;
        [SerializeField] private Color defaultColor = Color.white;
        [SerializeField] private Color touchedColor = Color.cyan;
        private HexMesh hexMesh;
        private Canvas gridCanvas;

        private HexCell[] cells;

        private void Awake() {
            gridCanvas = GetComponentInChildren<Canvas>();
            hexMesh = GetComponentInChildren<HexMesh>();
            cells = new HexCell[height * width];
            for (int z = 0, i = 0; z < height; z++) {
                for (var x = 0; x < width; x++) {
                    createCell(x, z, i++);
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
            (cellTransform = cell.transform).SetParent(transform, false);
            cellTransform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.color = defaultColor;

            TMP_Text label = Instantiate(cellLabelPrefab, gridCanvas.transform, false);
            label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
            label.text = cell.coordinates.toStringOnSeparateLines();
        }

        // Start is called before the first frame update
        void Start() {
            hexMesh.triangulate(cells);
        }

        // Update is called once per frame
        void Update() {
            if (Input.GetMouseButton(0)) {
                handleInput();
            }
        }

        private void handleInput() {
            var inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(inputRay, out var hit)) {
                touchCell(hit.point);
            }
        }

        private void touchCell(Vector3 position) {
            position = transform.InverseTransformPoint(position);
            var coordinates = HexCoordinates.FromPosition(position);
            Debug.Log("touched at " + coordinates);
            var index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
            var cell = cells[index];
            cell.color = touchedColor;
            hexMesh.triangulate(cells);
        }
    }
}