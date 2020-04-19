using GameEventSystem;
using hex;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

// ReSharper disable RedundantDefaultMemberInitializer

namespace controllers {
    [RequireComponent(typeof(CharacterMovement))]
    public class CamelController : MonoBehaviour {
        [SerializeField] private GameObject target = default;

        private NavMeshAgent navMeshAgent;
        [SerializeField] private GameEvent finishedTurn = default;
        [SerializeField] private int maxDistance = 2;
        [SerializeField] private float animationSpeed = 4;
        [SerializeField] private int everyNTurns = 2;
        [SerializeField] private GameObject tooltip = default;
        private int currentNbTurns = 0;

        private float realMaxDistance;
        private CharacterMovement characterMovement;
        private Animator animator;
        private static readonly int WALK = Animator.StringToHash("walk");
        private static readonly int STOP = Animator.StringToHash("stop");
        [SerializeField] private GameEvent camelClicked = default;

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

            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.speed = animationSpeed;
        }

        [UsedImplicitly]
        // Used by the turn manager
        public void DoYourTurn() {
            Debug.Log("Woohoo camel turn!");
            if (currentNbTurns >= everyNTurns) {
                currentNbTurns = 0;
                finishedTurn.Raise();
                return;
            }

            currentNbTurns++;
            var goTo = target.transform.position;
            goTo.y = transform.position.y;
            animator.SetTrigger(WALK);
            characterMovement.navigateTo(goTo, realMaxDistance, () => {
                animator.SetTrigger(STOP);
                finishedTurn.Raise();
            });
        }

        private void OnMouseDown() {
            camelClicked.Raise();
        }

        private void OnMouseEnter() {
            tooltip.SetActive(true);
        }

        private void OnMouseExit() {
            tooltip.SetActive(false);
        }
    }
}