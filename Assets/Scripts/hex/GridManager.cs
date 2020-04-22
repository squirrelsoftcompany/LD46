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
        private int _width = 6;
        private int _height = 6;

        [SerializeField] private NavMeshSurface navMeshSurface = default;

        [SerializeField] private GameEvent clickedCell = default;
        private Camera mainCamera;


        [SerializeField] private GameObject _playerPrefab = default;
        [SerializeField] private GameObject _camelPrefab = default;
        [SerializeField] private GameObject _wolfPrefab = default;

        [SerializeField] private Transform _alliesParent = default;
        [SerializeField] private Transform _enemiesParent = default;

        public Grid myGrid => _grid;

        private void Awake() {
            _grid = new Grid();

            LevelManager.Instance.GetLevelData(out var buildings, out var lakes, out var grounds, out var props, out _width, out _height, out var cellPrefab);
            var generator = new GridGenerator(myGrid, buildings, lakes, grounds, props, _width, _height, cellPrefab,
                cellsGameObject.transform, DateTime.Now.Millisecond);
            generator.Generate();

            navMeshSurface.BuildNavMesh();

            /// place living creatures

            var turnMgr = FindObjectOfType<Turn.TurnManager>();

            // PLAYER
            var p = _grid.GetRandomBorderCell();
            while (!_grid.CellAvailable(p))
            {
                p = _grid.GetRandomBorderCell();
            }
            var player = Instantiate(_playerPrefab, _alliesParent);
            player.transform.localPosition = p.ToPosition();
            //_grid[p].topping = player;

            // CAMEL
            var c = p;
            foreach (var n in ExtensionsHex.GetConvexFormAround(p, 1, new uint[] {0,0,0}))
            {
                c = n;
                if (_grid.CellAvailable(n))
                    continue;
            }
            var camel = Instantiate(_camelPrefab, _alliesParent);
            camel.transform.localPosition = c.ToPosition();
            //_grid[c].topping = camel;

            // WOLVES
            int wolvesCount = 5;
            for (int i = 0; i < wolvesCount; i++)
            {
                var w = _grid.GetRandomAvailableCell();
                var wolf = Instantiate(_wolfPrefab, _enemiesParent);
                wolf.transform.localPosition = w.ToPosition();
                //_grid[w].topping = wolf;
            }

            // Set camera now (it should exists now ^^")
            mainCamera = Camera.main;

            // navMeshSurface.BuildNavMesh();
            // planeCollisions.transform.localScale = new Vector3(width * HexMetrics.innerRadius, 1, 
            //     height*HexMetrics.outerRadius);
        }

        private void Start() {
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
                HexCoordinates.FromOffsetCoordinates(0, _height - 1).ToPosition());
            Gizmos.DrawLine(HexCoordinates.FromOffsetCoordinates(0, 0).ToPosition(),
                HexCoordinates.FromOffsetCoordinates(_width - 1, 0).ToPosition());
            Gizmos.DrawLine(HexCoordinates.FromOffsetCoordinates(_width - 1, _height - 1).ToPosition(),
                HexCoordinates.FromOffsetCoordinates(0, _height - 1).ToPosition());
            Gizmos.DrawLine(HexCoordinates.FromOffsetCoordinates(_width - 1, _height - 1).ToPosition(),
                HexCoordinates.FromOffsetCoordinates(_width - 1, 0).ToPosition());
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