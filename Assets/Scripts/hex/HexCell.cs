using System;
using UnityEngine;

namespace hex {
    public class HexCell : MonoBehaviour {
        public HexCoordinates coordinates;
        public CellType type; // TODO to change

        [SerializeField] private Color hover, normal;

        [SerializeField] private MeshRenderer meshRenderer;
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
    }
}