using UnityEngine;

namespace hex {
    // ReSharper disable RedundantDefaultMemberInitializer

    public class HexCell : MonoBehaviour {
        public HexCoordinates coordinates;

        public GroundType type;

        public GameObject ground;
        public GameObject topping;

        [SerializeField] private Color hover = default, normal = default;

        [SerializeField] private MeshRenderer meshRenderer = default;
        // private Material meshHover;
        // private Material meshNormal;

        public bool IsHovered { get; set; }

        public void initMesh() {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        // private void OnEnable() {
        // meshRenderer = GetComponentInChildren<MeshRenderer>();
        // }

        public void setHighlighted() {
            meshRenderer.material.color = hover;
            IsHovered = true;
        }

        public void setNotHighlighted() {
            meshRenderer.material.color = normal;
            IsHovered = false;
        }

        public bool available() {
            return topping == null;
        }

        public void moveToppingTo(HexCell other) {
            if (other == null || !other.available()) return;

            other.topping = topping;
            topping = null;
        }
    }
}