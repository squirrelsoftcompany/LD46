using GameEventSystem;
using hex;
using JetBrains.Annotations;
using UnityEngine;

// ReSharper disable RedundantDefaultMemberInitializer

namespace controllers {
    [RequireComponent(typeof(CharacterMovement))]
    public class EnemyController : MonoBehaviour {
        [SerializeField] private GameObject target = default;
        [SerializeField] private GameEvent finishedTurn = default;
        private float attackRange = 1;
        [SerializeField] private float maxDistanceTravel = 1;

        private CharacterMovement characterMovement;

        private void Awake() {
            if (target == null) {
                target = GameObject.FindWithTag("Camel");
                if (target == null) {
                    Debug.LogWarning("/!\\ No camel in this scene !!");
                }
            }

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

            // Else travel
            // calculate nav route TODO to change this
            if (hexTarget.DistanceTo(characterMovement.Position) <= maxDistanceTravel) {
                characterMovement.moveTo(hexTarget, finishedTurn);
            }

            // TODO to change to the next available tile in route within speed distance
            characterMovement.moveTo(hexTarget, finishedTurn);
        }

        private void attack() {
            // TODO attack the target
        }
    }
}