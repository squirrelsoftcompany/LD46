﻿using GameEventSystem;
using hex;
using JetBrains.Annotations;
using UnityEngine;
// ReSharper disable RedundantDefaultMemberInitializer

namespace controllers {
    [RequireComponent(typeof(CharacterMovement))]
    public class PlayerController : MonoBehaviour {
        private CharacterMovement characterMovement;
        [SerializeField] private float maxDistance = 1;
        [SerializeField] private GameEvent turnFinished = default;
        [SerializeField] private float speedAnimation;
        private bool myTurn = true;

        private void Awake() {
            characterMovement = GetComponent<CharacterMovement>();
        }


        [UsedImplicitly]
        public void onClickedCell(MonoBehaviour monoBehaviour) {
            turnFinished.sentString = "turnDone";
            // It is not my turn, so do nothing
            // TODO uncomment this when the turn manager will be up and running! (now for debug, commented)
            if (!myTurn) return;

            // We retrieve the cell to go to
            var hexCell = monoBehaviour.GetComponent<HexCell>();
            if (characterMovement.Position.Equals(hexCell.coordinates)) {
                // Just stay here
                myTurn = false;
                turnFinished.Raise();
                return;
            }

            // If within range, then go there
            if (characterMovement.Position.DistanceTo(hexCell.coordinates) <= maxDistance) {
                // and at the end, set myTurn to false
                characterMovement.moveTo(hexCell.coordinates, speedAnimation, turnFinished, () => myTurn = false);
            }
        }

        // My turn to do things (the turn manager said so)
        [UsedImplicitly]
        public void DoYourTurn() {
            myTurn = true;
        }
    }
}