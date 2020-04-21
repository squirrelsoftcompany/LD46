using System;
using UnityEngine;

namespace hex {
    // ReSharper disable RedundantDefaultMemberInitializer

    public class HexCell : MonoBehaviour {
        public HexCoordinates coordinates;

        public GroundType type;

        public GameObject ground;
        public GameObject topping;

        [SerializeField] private Color hover = default,
            normal = default,
            current = default,
            invalid = default,
            affected = default;

        [SerializeField] private MeshRenderer meshRenderer = default;

        // private Material meshHover;
        // private Material meshNormal;

        public bool IsHovered { get; set; }
        private Highlight highlight;

        public Highlight Highlight {
            get => highlight;
            set {
                if (highlight == value) return;
                highlight = value;
                setHighlight(highlight);
            }
        }

        public void initMesh() {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
            normal = meshRenderer.material.color;
        }

        private void setHighlight(Highlight highlightLevel) {
            Color color;
            switch (highlightLevel) {
                case Highlight.NORMAL:
                    color = normal;
                    IsHovered = false;
                    break;
                case Highlight.CURRENT_ACTION:
                    color = current;
                    IsHovered = false;
                    break;
                case Highlight.HIGHLIGHTED:
                    color = hover;
                    IsHovered = true;
                    break;
                case Highlight.INVALID:
                    color = invalid;
                    IsHovered = false;
                    break;
                case Highlight.AFFECTED:
                    color = affected;
                    IsHovered = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(highlightLevel), highlightLevel, null);
            }

            meshRenderer.material.color = color;
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

public enum Highlight {
    NORMAL,
    CURRENT_ACTION,
    HIGHLIGHTED,
    INVALID,
    AFFECTED
}