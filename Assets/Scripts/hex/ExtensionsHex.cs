using System.Collections.Generic;
using UnityEngine;

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

        public static List<HexCoordinates> getCellsAround(this HexCoordinates hexCoordinates, int radius) {
            // TODO get cells around in a radius of radius 
            Debug.LogError("getCellsAround not implemented!!!");
            var around = new List<HexCoordinates> {hexCoordinates, hexCoordinates.Xplus(), hexCoordinates.Xminus()};
            return around;
            // throw new NotImplementedException("TODO");
        }
    }
}