using System.Collections.Generic;
using GameEventSystem;
using UnityEditor;
using UnityEngine;

namespace hex {
    public class Grid {
        private Dictionary<HexCoordinates, HexCell> _grid;

        public Dictionary<HexCoordinates, HexCell> InternalGrid { get{ return _grid; } }

        public Grid()
        {
            _grid = new Dictionary<HexCoordinates, HexCell>();
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

        public HexCoordinates GetRandomAvailableCell()
        {
            Debug.Assert(_grid.Count > 0);
            var c = _grid.ElementAt(Random.Range(0, _grid.Count)).Key;
            while (!CellAvailable(c))
            {
                c = _grid.ElementAt(Random.Range(0, _grid.Count)).Key;
            }
            return c;
        }
    }
}