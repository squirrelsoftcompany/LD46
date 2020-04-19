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
        private int currentNbTurns = 0;

        private float realMaxDistance;
        private CharacterMovement characterMovement;

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
            characterMovement.navigateTo(goTo, realMaxDistance, () => finishedTurn.Raise());
        }
    }
}