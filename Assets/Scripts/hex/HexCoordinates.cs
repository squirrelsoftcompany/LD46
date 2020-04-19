using System;
using UnityEngine;

namespace hex {
    [Serializable]
    public struct HexCoordinates : IComparable<HexCoordinates> {
        [SerializeField] private int x;
        [SerializeField] private int z;

        public int X => x;

        public int Y => -X - Z;

        public int Z => z;

        public HexCoordinates(int x, int z) {
            this.x = x;
            this.z = z;
        }

        public static HexCoordinates FromOffsetCoordinates(int x, int z) {
            return new HexCoordinates(x - z / 2, z);
        }

        public override string ToString() {
            return "(" + X + ", " + Y + ", " + Z + ")";
        }

        public string toStringOnSeparateLines() {
            return X + "\n" + Y + "\n" + Z;
        }

        public static HexCoordinates FromPosition(Vector3 position) {
            var x = position.x / (HexMetrics.innerRadius * 2f);
            var y = -x;
            var offset = position.z / (HexMetrics.outerRadius * 3f);
            x -= offset;
            y -= offset;
            var iX = Mathf.RoundToInt(x);
            var iY = Mathf.RoundToInt(y);
            var iZ = Mathf.RoundToInt(-x - y);
            if (iX + iY + iZ == 0) return new HexCoordinates(iX, iZ);

            var dX = Mathf.Abs(x - iX);
            var dY = Mathf.Abs(y - iY);
            var dZ = Mathf.Abs(-x - y - iZ);
            if (dX > dY && dX > dZ) {
                iX = -iY - iZ;
            } else if (dZ > dY) {
                iZ = -iX - iY;
            }

            return new HexCoordinates(iX, iZ);
        }

        public Vector3 ToPosition() {
            int offsetZ = z;
            // x = ox - oz / 2
            // x - ox = - oz / 2
            // - ox = - x -oz / 2
            // ox = x + oz / 2
            int offsetX = x + Mathf.FloorToInt(offsetZ / 2.0f);

            Vector3 position;
            position.x = (offsetX + offsetZ * 0.5f - offsetZ / 2) * HexMetrics.innerRadius * 2f;
            position.y = 0f;
            position.z = offsetZ * HexMetrics.outerRadius * 1.5f;

            return position;
        }

        public int CompareTo(HexCoordinates other) {
            var compX = X.CompareTo(other.X);
            if (compX != 0) {
                return compX;
            }

            return Z.CompareTo(other.Z);
        }

        public int DistanceTo(HexCoordinates other) {
            return ((x < other.x ? other.x - x : x - other.x) +
                    (Y < other.Y ? other.Y - Y : Y - other.Y) +
                    (z < other.z ? other.z - z : z - other.z)) / 2;
        }

        public HexCoordinates moveAlongX(int offset)
        {
            var result = new HexCoordinates(X, Z);
            result.x = result.x + offset;
            return result;
        }

        public HexCoordinates moveAlongZ(int offset)
        {
            var result = new HexCoordinates(X, Z);
            result.z = result.z + offset;
            return result;
        }

        public HexCoordinates moveAlongY(int offset) => moveAlongX(-offset).moveAlongZ(-offset);

        public HexCoordinates Xplus() => moveAlongX(1);
        public HexCoordinates Xminus() => moveAlongX(-1);
        public HexCoordinates Zplus() => moveAlongZ(1);
        public HexCoordinates Zminus() => moveAlongZ(-1);
        public HexCoordinates Yplus() => moveAlongY(1);
        public HexCoordinates Yminus() => moveAlongY(-1);
    }
}