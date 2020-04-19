using System;
using GameEventSystem;
using hex;
using JetBrains.Annotations;
using UnityEngine;

namespace controllers {
    [RequireComponent(typeof(CharacterMovement))]
    public class EnemyController : MonoBehaviour {
        [SerializeField] private GameObject target;
        [SerializeField] private GameEvent finishedTurn;
        private float attackRange = 1;
        [SerializeField] private float maxDistanceTravel = 1;

        private CharacterMovement characterMovement;

        private void Awake() {
            characterMovement = GetComponent<CharacterMovement>();
        }

        // Method called by TurnManager, do not rename
        [UsedImplicitly]
        public void DoYourTurn() {
            // find position to go to
            var targetPos = target.transform.position;
            // if within range of attack, then attack
            var hexTarget = HexCoordinates.FromPosition(targetPos);
            if (hexTarget.DistanceTo(characterMovement.Position) <= attackRange) {
                attack();
                finishedTurn.sentString = "Enemy attack";
                finishedTurn.Raise();
                return;
            }
            // Else travel
            // calculate nav route

            // TODO to change to the next available tile in route within speed distance
            finishedTurn.sentString = "enemy move";
            characterMovement.moveTo(hexTarget, finishedTurn);
        }

        private void attack() {
            // TODO attack the target
        }
    }
}