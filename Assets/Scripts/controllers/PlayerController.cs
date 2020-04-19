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
        [SerializeField] private float speedAnimation = 4;
        private bool myTurn = true;
        private Animator animator;
        private static readonly int STOP = Animator.StringToHash("stop");
        private static readonly int WALK = Animator.StringToHash("walk");

        private void Awake() {
            characterMovement = GetComponent<CharacterMovement>();
            animator = GetComponentInChildren<Animator>();
        }


        [UsedImplicitly]
        public void onClickedCell(MonoBehaviour monoBehaviour) {
            turnFinished.sentString = "turnDone";
            // It is not my turn, so do nothing
            // TODO uncomment this when the turn manager will be up and running! (now for debug, commented)
            if (!myTurn) return;
            Debug.Log("Now, we're talking movement!");
            myTurn = false;
            // We retrieve the cell to go to
            var hexCell = monoBehaviour.GetComponent<HexCell>();
            if (characterMovement.Position.Equals(hexCell.coordinates)) {
                // Just stay here
                turnFinished.Raise();
                return;
            }

            // If within range, then go there
            if (characterMovement.Position.DistanceTo(hexCell.coordinates) <= maxDistance) {
                // and at the end, set myTurn to false
                animator.SetTrigger(WALK);
                characterMovement.moveTo(hexCell.coordinates, speedAnimation, () => {
                    animator.SetTrigger(STOP);
                    turnFinished.Raise();
                });
            }
        }

        // My turn to do things (the turn manager said so)
        [UsedImplicitly]
        public void DoYourTurn() {
            myTurn = true;
            Debug.Log("It's my turn!");
        }
    }
}