using System.Linq;
using district;
using GameEventSystem;
using hex;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

// ReSharper disable RedundantDefaultMemberInitializer

namespace controllers {
    [RequireComponent(typeof(CharacterMovement))]
    public class PlayerController : MonoBehaviour {
        private CharacterMovement characterMovement;
        [SerializeField] private float maxDistance = 1;
        [SerializeField] private GameEvent turnFinished = default;
        [SerializeField] private float speedAnimation = 4;
        [SerializeField] private GameObject tooltip = default;
        [SerializeField] private int noisePower = 3;
        private TMP_Text inventoryText;
        private GridManager gridManager;
        private bool myTurn = true;
        private Animator animator;
        private NavMeshAgent navMeshAgent;
        private static readonly float STOPPING_DISTANCE_TARGET = 1.realDistanceFromHexDistance() / 4;
        private static readonly int STOP = Animator.StringToHash("stop");
        private static readonly int WALK = Animator.StringToHash("walk");
        private static readonly int NOISE = Animator.StringToHash("noise");

        private Flee enemies;
        [SerializeField] private int distanceTransfer = 1;
        private Census census;
        [SerializeField] private int food, water;
        [SerializeField] private int maxWeight = 10;
        [SerializeField] private int distanceFouille = 1;
        private bool isFull => water + food >= maxWeight;

        private void Awake() {
            characterMovement = GetComponent<CharacterMovement>();
            animator = GetComponentInChildren<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            enemies = FindObjectOfType<Flee>();
            navMeshAgent.speed = speedAnimation;
            census = FindObjectOfType<Census>();
            inventoryText = tooltip.GetComponentInChildren<TMP_Text>();
        }

        private void Start() {
            gridManager = FindObjectOfType<GridManager>();
            setInventoryText();
        }

        private void setInventoryText() {
            inventoryText.text = "Food:\t\t" + food + "\nWater:\t" + water;
        }

        [UsedImplicitly]
        public void onClickedCell(MonoBehaviour monoBehaviour) {
            // It is not my turn, so do nothing
            if (!myTurn) return;
            Debug.Log("Now, we're talking movement!");

            // We retrieve the cell to go to
            var hexCell = monoBehaviour.GetComponent<HexCell>();
            hexCell.Highlight = Highlight.CURRENT_ACTION;
            var distanceToMe = characterMovement.Position.DistanceTo(hexCell.coordinates);
            if (distanceToMe > maxDistance ||
                !gridManager.myGrid.CellAvailable(hexCell.coordinates) ||
                isOccupiedCell(hexCell.coordinates) && distanceToMe != 0
            ) {
                hexCell.Highlight = Highlight.NORMAL;
                return; // clicked on invalid cell, it is still my turn
            }

            myTurn = false;
            if (distanceToMe == 0) {
                // Just stay here
                hexCell.Highlight = Highlight.NORMAL;
                turnFinished.Raise();
                return;
            }

            // We are within range, so go there
            // and at the end, set myTurn to false
            animator.SetTrigger(WALK);
            characterMovement.navigateTo(hexCell.coordinates.ToPosition(),
                ((int) maxDistance).realDistanceFromHexDistance(),
                STOPPING_DISTANCE_TARGET,
                () => {
                    animator.SetTrigger(STOP);
                    hexCell.Highlight = Highlight.NORMAL;
                    Debug.Log("[Player] finished");
                    turnFinished.Raise();
                });
        }

        private bool isOccupiedCell(HexCoordinates cellCoordinates) {
            return census.isOccupiedCell(cellCoordinates);
        }

        private void doTransfer() {
            // transfer to camel
            InventoryManager.Instance.addFood(food);
            food = 0;
            InventoryManager.Instance.addWater(water);
            water = 0;
            setInventoryText();
            Debug.Log("Transfer to camel");
        }

        [UsedImplicitly]
        public void onClickedCamelTransfer(MonoBehaviour camel) {
            if (!myTurn) return;
            Debug.Log("[Player] click on camel! we want a transfer");
            var camelPos = camel.GetComponent<CamelController>().Position;
            if (characterMovement.Position.DistanceTo(camelPos) > distanceTransfer) {
                // Camel is too far 
                return;
            }

            myTurn = false;
            doTransfer();
            Debug.Log("[Player] finished");
            turnFinished.Raise();
        }

        public void onClickedBuilding(MonoBehaviour building) {
            if (!myTurn) return;
            var buildingInventory = building.GetComponent<BuildingInventory>();

            if (characterMovement.Position.DistanceTo(buildingInventory.Coordinates) > distanceFouille) {
                // Building is too far
                // TODO play error sound
                return;
            }

            if (isFull) {
                // can't take, too far
                // TODO play error sound
                return;
            }

            myTurn = false;
            water += buildingInventory.takeWaterUntil(maxWeight - food - water);
            food += buildingInventory.takeFoodUntil(maxWeight - food - water);
            setInventoryText();
            // TODO trigger take food animation, and call raise at the end of anim

            turnFinished.Raise();
        }

        // My turn to do things (the turn manager said so)
        [UsedImplicitly]
        public void DoYourTurn() {
            myTurn = true;
            Debug.Log("[Player] It's my turn!");
        }

        private void OnMouseDown() {
            if (!myTurn) return;
            myTurn = false;
            // Make noise
            animator.SetTrigger(NOISE);
            enemies.flee(characterMovement.Position, noisePower);
            Debug.Log("[Player] Noise");
        }

        // Used by end animation noise
        public void onFinishedAnimationSound() {
            Debug.Log("[Player] finished");
            turnFinished.Raise();
        }

        private void OnMouseEnter() {
            tooltip.SetActive(true);
            enemies.displayNoise(noisePower, characterMovement.Position, true);
            var affectedCells = characterMovement.Position.GetDiskAround((uint) noisePower)
                .Where(coordinates => gridManager.myGrid[coordinates] != null);
            foreach (var affectedCell in affectedCells) {
                gridManager.myGrid[affectedCell].Highlight = Highlight.AFFECTED;
            }
        }

        private void OnMouseExit() {
            tooltip.SetActive(false);
            enemies.displayNoise(noisePower, characterMovement.Position, false);
            var affectedCells = characterMovement.Position.GetDiskAround((uint) noisePower)
                .Where(coordinates => gridManager.myGrid[coordinates] != null);
            foreach (var affectedCell in affectedCells) {
                gridManager.myGrid[affectedCell].Highlight = Highlight.NORMAL;
            }
        }
    }
}