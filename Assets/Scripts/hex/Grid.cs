using System.Collections.Generic;
using GameEventSystem;
using UnityEditor;
using UnityEngine;

namespace hex {
    public class Grid {
        private Dictionary<HexCoordinates, HexCell> _grid;

        public Grid(int width, int height, GameObject cellPrefab, Transform transformParent) {
            _grid = new Dictionary<HexCoordinates, HexCell>();
            for (int z = 0; z < height; z++) {
                for (var x = 0; x < width; x++) {
                    HexCoordinates c = HexCoordinates.FromOffsetCoordinates(x, z);
                    InitCell(c, cellPrefab, transformParent);
                }
            }

            //var generator = new GridGenerator(cellsGameObject.transform, this, );
            //generator.Generate();
        }

        private void InitCell(HexCoordinates c, GameObject cellPrefab, Transform transformParent) {
            var go = GameObject.Instantiate(cellPrefab, transformParent, false);
            Debug.Assert(go);
            HexCell cell = go.GetComponent<HexCell>();
            Debug.Assert(cell);

            cell.transform.localPosition = c.ToPosition();
            cell.coordinates = c;
            _grid[cell.coordinates] = cell;
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

        public Dictionary<HexCoordinates, HexCell>.KeyCollection Keys => _grid.Keys;

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