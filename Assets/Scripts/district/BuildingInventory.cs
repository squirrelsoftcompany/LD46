using System;
using GameEventSystem;
using hex;
using TMPro;
using UnityEngine;

namespace district {
    public class BuildingInventory : MonoBehaviour {
        [SerializeField] private int food, water;
        [SerializeField] private Color hoveredColor = Color.green;
        private MeshRenderer model;
        private Camera mainCamera;
        [SerializeField] private GameObject tooltip;
        private GameObject childObject;
        [SerializeField] private GameEvent clickedBuilding;
        private TMP_Text text;

        public HexCoordinates Coordinates { get; private set; }

        private void Awake() {
            model = GetComponentInChildren<MeshRenderer>();
            childObject = model.gameObject;
            Coordinates = HexCoordinates.FromPosition(transform.position);
            text = tooltip.GetComponentInChildren<TMP_Text>(true);
            setInventoryText();
        }

        private void Start()
        {
            mainCamera = Camera.main;
        }

        public void initFoodWaterPosition(int iFood, int iWater) {
            food = iFood;
            water = iWater;
            setInventoryText();
        }

        private void setInventoryText() {
            text.text = "Food:\t\t" + food + "\nWater:\t" + water;
        }

        private void Update() {
            var inputRay = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(inputRay, out var hit)) return;
            var hitObject = hit.transform.gameObject;
            if (hitObject.Equals(gameObject) || hitObject.Equals(childObject)) {
                // We hovered this
                if (!tooltip.activeSelf) {
                    tooltip.SetActive(true);
                    model.material.color = hoveredColor;
                }

                if (!Input.GetMouseButton(0)) return;
                clickedBuilding.sentMonoBehaviour = this;
                clickedBuilding.Raise();
            } else {
                if (!tooltip.activeSelf) return;
                tooltip.SetActive(false);
                model.material.color = Color.white;
            }
        }

        public int takeFoodUntil(int maxFood) {
            var res = Math.Min(maxFood, food);

            food -= res;
            setInventoryText();
            return res;
        }

        public int takeWaterUntil(int maxWater) {
            var res = Math.Min(maxWater, water);

            water -= res;
            setInventoryText();
            return res;
        }
    }
}