using UnityEngine;

namespace hex {
    public static class HexMetrics {
        public const float outerRadius = 2f;
        public const float innerRadius = 1.7f;

        public static readonly Vector3[] cornersPointyTop = {
            new Vector3(0f, 0f, outerRadius),
            new Vector3(innerRadius, 0f, 0.5f * outerRadius),
            new Vector3(innerRadius, 0f, -0.5f * outerRadius),
            new Vector3(0f, 0f, -outerRadius),
            new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
            new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
            new Vector3(0f, 0f, outerRadius),
        };
    }
}