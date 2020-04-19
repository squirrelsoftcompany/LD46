using System;
using UnityEngine;

namespace hex {
    public static class ExtensionsHex {
        public static Vector3 newToPosition(this HexCoordinates coordinates) {
            int offsetZ = coordinates.Z;
            // x = ox - oz / 2
            // x - ox = - oz / 2
            // - ox = - x -oz / 2
            // ox = x + oz / 2
            int offsetX = coordinates.X + Mathf.FloorToInt(offsetZ / 2.0f);

            Vector3 position;
            position.x = (offsetX + offsetZ * 0.5f - offsetZ / 2) * HexMetrics.innerRadius * 2f;
            position.y = 0f;
            position.z = offsetZ * HexMetrics.outerRadius * 1.5f;

            return position;
        }

        public static int hexDistance(Vector3 position1, Vector3 position2) {
            return HexCoordinates.FromPosition(position1).DistanceTo(HexCoordinates.FromPosition(position2));
        }

        public static int hexDistanceFromDistance(float distance) {
            return hexDistance(Vector3.zero, Vector3.right * distance);
        }

        public static float realDistanceFromHexDistance(int hexDistance) {
            return (new HexCoordinates(0, 0).newToPosition() - new HexCoordinates(hexDistance, 0).newToPosition()).magnitude;
        }
    }
}