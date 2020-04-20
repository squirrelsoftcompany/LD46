using GameEventSystem;
using hex;
using UnityEngine;

namespace district {
    public class BuildingInventory : MonoBehaviour {
        [SerializeField] private int food, water;
        [SerializeField] private Color hoveredColor = Color.green;
        private MeshRenderer model;
        private Camera mainCamera;
        private GameObject childObject;
        [SerializeField] private GameEvent clickedBuilding;

        public HexCoordinates Coordinates { get; private set; }

        private void Awake() {
            model = GetComponentInChildren<MeshRenderer>();
            childObject = model.gameObject;
            mainCamera = Camera.main;
            Coordinates = HexCoordinates.FromPosition(transform.position);
        }

        public void initFoodWaterPosition(int iFood, int iWater, HexCoordinates iCoordinates) {
            food = iFood;
            water = iWater;
            Coordinates = iCoordinates;
        }

        private void Update() {
            var inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(inputRay, out var hit)) return;
            var hitObject = hit.transform.gameObject;
            if (hitObject.Equals(gameObject) || hitObject.Equals(childObject)) {
                // We hovered this
                model.material.color = hoveredColor;
                if (!Input.GetMouseButton(0)) return;
                clickedBuilding.sentMonoBehaviour = this;
                clickedBuilding.Raise();
            } else {
                model.material.color = Color.white;
            }
        }

        public int takeFoodUntil(int maxFood) {
            int res;
            if (maxFood > food) {
                res = food;
            } else {
                res = food - maxFood;
            }

            food -= res;
            return res;
        }

        public int takeWaterUntil(int maxWater) {
            int res;
            if (maxWater > water) {
                res = water;
            } else {
                res = water - maxWater;
            }

            water -= res;
            return res;
        }
    }
}