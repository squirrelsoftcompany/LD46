using System.Collections.Generic;
using System.Linq;
using GameEventSystem;
using UnityEditor;
using UnityEngine;
using static hex.HexCoordinates;

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

        public bool IsBorderCell(HexCoordinates c)
        {
            if (this[c] == null)
                return false;
            var disk = c.GetConvexFormAround(1, new uint[] {0,0,0});
            return disk.Where(x => this[x] != null).ToList().Count < 6;
        }
    }
}