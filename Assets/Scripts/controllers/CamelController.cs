using GameEventSystem;
using hex;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

// ReSharper disable RedundantDefaultMemberInitializer

namespace controllers {
    [RequireComponent(typeof(CharacterMovement))]
    public class CamelController : MonoBehaviour {
        [SerializeField] private GameObject target = default;

        private NavMeshAgent navMeshAgent;
        [SerializeField] private GameEvent finishedTurn = default;
        [SerializeField] private int maxDistance = 1;
        [SerializeField] private float animationSpeed = 4;
        [SerializeField] private int everyNTurns = 2;
        [SerializeField] private GameObject tooltip = default;
        private TMP_Text text;
        private int currentNbTurns = 0;

        private float realMaxDistance;
        private static readonly float STOPPING_DISTANCE_TARGET = 1.realDistanceFromHexDistance();
        private CharacterMovement characterMovement;
        private Animator animator;
        private static readonly int WALK = Animator.StringToHash("walk");
        private static readonly int STOP = Animator.StringToHash("stop");
        [SerializeField] private GameEvent camelClicked = default;

        public HexCoordinates Position => characterMovement.Position;

        private void Awake() {
            if (target == null) {
                target = GameObject.FindWithTag("Player");
                if (target == null) {
                    Debug.LogWarning("Cannot find player tagged with Player");
                }
            }

            if (finishedTurn == null) {
                Debug.LogWarning("FinishedTurn is not set");
            }

            characterMovement = GetComponent<CharacterMovement>();
            animator = GetComponentInChildren<Animator>();
            realMaxDistance = maxDistance.realDistanceFromHexDistance();

            text = tooltip.GetComponentInChildren<TMP_Text>(true);
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.speed = animationSpeed;
            navMeshAgent.stoppingDistance = 1.realDistanceFromHexDistance();
            setInventoryText();
        }

        private void setInventoryText() {
            text.text = "Food:\t\t" + InventoryManager.Instance.Food + "\nWater:\t" + InventoryManager.Instance.Water;
        }


        [UsedImplicitly]
        // Used by the turn manager
        public void DoYourTurn() {
            Debug.Log("[Camel] my turn!");
            if (currentNbTurns >= everyNTurns) {
                currentNbTurns = 0;
                Debug.Log("[Camel] finished (no move)");
                finishedTurn.Raise();
                return;
            }

            currentNbTurns++;
            var goTo = target.transform.position;
            goTo.y = transform.position.y;
            animator.SetTrigger(WALK);
            characterMovement.navigateTo(goTo, realMaxDistance, STOPPING_DISTANCE_TARGET, () => {
                animator.SetTrigger(STOP);
                Debug.Log("[Camel] Finished");
                finishedTurn.Raise();
            });
        }

        private void OnMouseDown() {
            camelClicked.sentMonoBehaviour = this;
            camelClicked.Raise();
            setInventoryText();
        }

        private void OnMouseEnter() {
            setInventoryText();
            tooltip.SetActive(true);
        }

        private void OnMouseExit() {
            tooltip.SetActive(false);
        }
    }
}