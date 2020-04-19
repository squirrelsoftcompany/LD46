using System;
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

        private void Awake() {
            // meshNormal = normal.GetComponent<Material>();
            // meshHover = hover.GetComponent<Material>();
            // meshCollider.material = meshNormal;
        }

        public void setHighlighted() {
            meshRenderer.material.color = hover;
        }

        public void setNotHighlighted() {
            meshRenderer.material.color = normal;
        }

        public bool available()
        {
            return topping == null;
        }

        public void moveToppingTo(HexCell other)
        {
            if (other == null || !other.available()) return;

            other.topping = topping;
            topping = null;
        }
    }
}