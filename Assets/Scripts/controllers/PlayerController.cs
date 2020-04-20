﻿using GameEventSystem;
using hex;
using JetBrains.Annotations;
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
        private bool myTurn = true;
        private Animator animator;
        private NavMeshAgent navMeshAgent;
        private static readonly int STOP = Animator.StringToHash("stop");
        private static readonly int WALK = Animator.StringToHash("walk");
        private static readonly int NOISE = Animator.StringToHash("noise");

        private Flee enemies;

        private void Awake() {
            characterMovement = GetComponent<CharacterMovement>();
            animator = GetComponentInChildren<Animator>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            enemies = FindObjectOfType<Flee>();
            navMeshAgent.speed = speedAnimation;
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
            hexCell.Highlight = Highlight.CURRENT_ACTION;
            if (characterMovement.Position.Equals(hexCell.coordinates)) {
                // Just stay here
                hexCell.Highlight = Highlight.NORMAL;
                turnFinished.Raise();
                return;
            }

            // If within range, then go there
            if (characterMovement.Position.DistanceTo(hexCell.coordinates) <= maxDistance) {
                // and at the end, set myTurn to false
                animator.SetTrigger(WALK);
                characterMovement.navigateTo(hexCell.coordinates.ToPosition(),
                    ((int) maxDistance).realDistanceFromHexDistance(),
                    () => {
                        animator.SetTrigger(STOP);
                        hexCell.Highlight = Highlight.NORMAL;
                        Debug.Log("[Player] finished");
                        turnFinished.Raise();
                    });
            } else {
                Debug.LogWarning("!!!!!!");
            }
        }

        [UsedImplicitly]
        public void onClickedCamelTransfer() {
            if (!myTurn) return;
            Debug.Log("[Player] click on camel! we want a transfer TODO");
            myTurn = false;
            // TODO transfer to camel
            Debug.Log("[Player] finished");
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
            // TODO Make noise
            myTurn = false;
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
        }

        private void OnMouseExit() {
            tooltip.SetActive(false);
            enemies.displayNoise(noisePower, characterMovement.Position, false);
        }
    }
}