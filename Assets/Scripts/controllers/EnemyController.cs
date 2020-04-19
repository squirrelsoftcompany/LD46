using GameEventSystem;
using hex;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

// ReSharper disable RedundantDefaultMemberInitializer

namespace controllers {
    [RequireComponent(typeof(CharacterMovement))]
    public class EnemyController : MonoBehaviour {
        [SerializeField] private GameObject target = default;
        [SerializeField] private GameEvent finishedTurn = default;
        private float attackRange = 1;
        [SerializeField] private int maxDistanceTravel = 1;
        [SerializeField] private float speedAnimation = 4;

        private CharacterMovement characterMovement;
        private NavMeshAgent navMeshAgent;

        private void Awake() {
            if (target == null) {
                target = GameObject.FindWithTag("Camel");
                if (target == null) {
                    Debug.LogWarning("/!\\ No camel in this scene !!");
                }
            }

            navMeshAgent = GetComponent<NavMeshAgent>();
            characterMovement = GetComponent<CharacterMovement>();
        }

        // Method called by TurnManager, do not rename
        [UsedImplicitly]
        public void DoYourTurn() {
            finishedTurn.sentString = "endTurn";
            // find position to go to
            var targetPos = target.transform.position;
            // if within range of attack, then attack
            var hexTarget = HexCoordinates.FromPosition(targetPos);
            if (hexTarget.DistanceTo(characterMovement.Position) <= attackRange) {
                attack();
                finishedTurn.Raise();
                return;
            }

            navMeshAgent.speed = speedAnimation;
            targetPos.y = transform.position.y;

            characterMovement.navigateTo(
                targetPosition: targetPos,
                realMaxDistance: maxDistanceTravel.realDistanceFromHexDistance(),
                onFinished: () => finishedTurn.Raise());
        }

        private void attack() {
            // TODO attack the target
        }
    }
}