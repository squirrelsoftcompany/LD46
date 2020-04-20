using System.Collections.Generic;
using UnityEngine;
using SixWay = hex.HexCoordinates.SixWay;

namespace hex {
    public static class ExtensionsHex {
        public static int hexDistance(Vector3 position1, Vector3 position2) {
            return HexCoordinates.FromPosition(position1).DistanceTo(HexCoordinates.FromPosition(position2));
        }

        public static int hexDistanceFromDistance(float distance) {
            return hexDistance(Vector3.zero, Vector3.right * distance);
        }

        public static HexCoordinates toHex(this Vector3 position) {
            return HexCoordinates.FromPosition(position);
        }

        public static float realDistanceFromHexDistance(this int hexDistance) {
            return (new HexCoordinates(0, 0).ToPosition() - new HexCoordinates(hexDistance, 0).ToPosition()).magnitude;
        }

        public static HexCoordinates oppositeTo(this HexCoordinates myPosition, HexCoordinates pivotPosition) {
            var diffX = myPosition.X - pivotPosition.X;
            var diffZ = myPosition.Z - pivotPosition.Z;
            return new HexCoordinates(pivotPosition.X - diffX, pivotPosition.Z - diffZ);
        }

        public static List<HexCoordinates> GetDiskAround(this HexCoordinates c, uint radius)
        {
            List<HexCoordinates> result = new List<HexCoordinates>();
            for (uint i = 0; i <= radius; i++)
            {
                var l = GetConvexFormAround(c, i, new uint[] {0,0,0});
                result.AddRange(l);
            }
            return result;
        }

        public static List<HexCoordinates> GetFilledConvexFormAround(this HexCoordinates c, uint radius, uint[] offsetByAxis, HexCoordinates.SixWay firstWallOrientation = HexCoordinates.SixWay.XMinus)
        {
            List<HexCoordinates> result = new List<HexCoordinates>();
            for (uint i = 0; i <= radius; i++)
            {
                var l = GetConvexFormAround(c, i, offsetByAxis, firstWallOrientation);
                result.AddRange(l);
            }
            return result;
        }

        public static List<HexCoordinates> GetConvexFormAround(this HexCoordinates c, uint radius, uint[] offsetByAxis, HexCoordinates.SixWay firstWallOrientation = HexCoordinates.SixWay.XMinus)
        {
            if (radius == 0 && offsetByAxis[0] == 0 && offsetByAxis[1] == 0 && offsetByAxis[2] == 0)
                return new List<HexCoordinates> {c};

            Debug.Assert(offsetByAxis.Length == 3);

            List<HexCoordinates> coords = new List<HexCoordinates>();

            hex.HexCoordinates begin = c.moveAlongAxis((SixWay)(((int)firstWallOrientation + 4) % 6), (int)radius);

            SixWay way = firstWallOrientation;
            var current = begin;
            for (int j = 0; j < 6; j++)
            {
                uint max = radius + offsetByAxis[j % 3];
                for (int i = 0; i < max; i++)
                {
                    current = current.moveAlongAxis(way, 1);
                    coords.Add(current);
                }

                way = (SixWay)(((int)way + 1) % 6);
            }

            Debug.Assert(begin.CompareTo(current) == 0);

            return coords;
        }
    }
}