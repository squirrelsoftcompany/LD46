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

        public Vector3 ToPosition()
        {
            Vector3 position = Vector3.zero;
            position.x = (z / 2.0f + X) * (HexMetrics.innerRadius * 2f);
            position.z = Z * (HexMetrics.outerRadius * 1.5f);
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

        /**
         * Move along current X axis, doesn't modify x coords but modify the two others
         */
        public HexCoordinates moveAlongX(int offset)
        {
            var result = new HexCoordinates(X, Z);
            result.z = result.z + offset;
            return result;
        }

        /**
         * Move along current Z axis, doesn't modify z coords but modify the two others
         */
        public HexCoordinates moveAlongZ(int offset)
        {
            var result = new HexCoordinates(X, Z);
            result.x = result.x + offset;
            return result;
        }

        /**
         * Move along current Y axis, doesn't modify y coords but modify the two others
         */
        public HexCoordinates moveAlongY(int offset) => moveAlongX(-offset).moveAlongZ(offset);

        /**
         * Move along current X axis by 1 cell, doesn't modify x coords but modify the two others
         */
        public HexCoordinates Xplus() => moveAlongX(1);
        /**
         * Move along current X axis by -1 cell, doesn't modify x coords but modify the two others
         */
        public HexCoordinates Xminus() => moveAlongX(-1);
        /**
         * Move along current Z axis by 1 cell, doesn't modify z coords but modify the two others
         */
        public HexCoordinates Zplus() => moveAlongZ(1);
        /**
         * Move along current Z axis by -1 cell, doesn't modify z coords but modify the two others
         */
        public HexCoordinates Zminus() => moveAlongZ(-1);
        /**
         * Move along current Y axis by 1 cell, doesn't modify y coords but modify the two others
         */
        public HexCoordinates Yplus() => moveAlongY(1);
        /**
         * Move along current Y axis by -1 cell, doesn't modify y coords but modify the two others
         */
        public HexCoordinates Yminus() => moveAlongY(-1);
    }
}