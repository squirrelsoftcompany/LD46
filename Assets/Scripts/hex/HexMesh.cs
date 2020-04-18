using System.Collections.Generic;
using UnityEngine;

namespace hex {
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour {
        [SerializeField] private Mesh hexMesh;
        private List<Vector3> vertices;
        private List<int> triangles;
        private List<Color> colors;
        private MeshCollider meshCollider;

        private void Awake() {
            GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
            meshCollider = gameObject.AddComponent<MeshCollider>();
            hexMesh.name = "Hex Mesh";
            colors = new List<Color>();
            vertices = new List<Vector3>();
            triangles = new List<int>();
        }

        public void triangulate(HexCell[] cells) {
            hexMesh.Clear();
            vertices.Clear();
            triangles.Clear();
            colors.Clear();
            foreach (var cell in cells) {
                triangulate(cell);
            }

            hexMesh.vertices = vertices.ToArray();
            hexMesh.colors = colors.ToArray();
            hexMesh.triangles = triangles.ToArray();
            hexMesh.RecalculateNormals();
            meshCollider.sharedMesh = hexMesh;
        }

        private void addTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
            var vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        private void addTriangleColor(Color color) {
            colors.Add(color);
            colors.Add(color);
            colors.Add(color);
        }

        private void triangulate(HexCell cell) {
            var center = cell.transform.localPosition;
            for (var i = 0; i < 6; i++) {
                addTriangle(center, center + HexMetrics.cornersPointyTop[i],
                    center + HexMetrics.cornersPointyTop[i + 1]);
                addTriangleColor(cell.color);
            }
        }
    }
}