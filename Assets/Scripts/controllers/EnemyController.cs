﻿using GameEventSystem;
using gauge;
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
        [SerializeField] private float attackRange = 1;
        [SerializeField] private int maxDistanceTravel = 1;
        [SerializeField] private int maxDistanceFlee = 1;
        [SerializeField] private float speedAnimation = 4;
        [SerializeField] private Color colorAffected = new Color32(255, 127, 0, 255);

        private static readonly float STOPPING_DISTANCE_TARGET = 1.realDistanceFromHexDistance();
        private GridManager gridManager;
        private CharacterMovement characterMovement;
        private NavMeshAgent navMeshAgent;
        private LifeGauge lifeOfMyEnemy;
        [SerializeField] private int attackPower = 1;
        private int fleeingTurn ;
        private HexCoordinates fleeTarget;
        private MeshRenderer[] meshRenderers;
        private Animator animator;
        private static readonly int AGGRESSIVE = Animator.StringToHash("Aggressive");
        private static readonly int ATTACK = Animator.StringToHash("attack");
        private static readonly int MOVE = Animator.StringToHash("move");
        private static readonly int STOP = Animator.StringToHash("Stop");

        private void Awake() {
            if (target == null) {
                target = GameObject.FindWithTag("Camel");
                if (target == null) {
                    Debug.LogWarning("/!\\ No camel in this scene !!");
                }
            }

            meshRenderers = GetComponentsInChildren<MeshRenderer>();
            lifeOfMyEnemy = target.GetComponent<LifeGauge>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.speed = speedAnimation;
            navMeshAgent.stoppingDistance = 1.realDistanceFromHexDistance();
            characterMovement = GetComponent<CharacterMovement>();
            characterMovement.Target = target.transform;
            gridManager = FindObjectOfType<GridManager>();
            animator = GetComponentInChildren<Animator>();
        }

        private void Start() {
            var turnMgr = FindObjectOfType<Turn.TurnManager>();
            turnMgr.AddWolf(this.gameObject);
        }

        // Method called by TurnManager, do not rename
        [UsedImplicitly]
        public void DoYourTurn() {
            Debug.Log("[Enemy] my turn");

            // if fleeing, then flee, and stop fleeing
            if (fleeingTurn>0) {
                doFlee(fleeingTurn);
                return;
            }

            // find position to go to
            var targetPos = target.transform.position;
            // if within range of attack, then attack
            var hexTarget = HexCoordinates.FromPosition(targetPos);
            if (hexTarget.DistanceTo(characterMovement.Position) <= attackRange) {
                animator.SetBool(AGGRESSIVE, true);
                animator.SetTrigger(ATTACK);
                attack();
                Invoke(nameof(notAggressive), 1);

                return;
            }

            targetPos.y = transform.position.y;

            gridManager.myGrid[characterMovement.Position].topping = null;
            Debug.Log("[Enemy] travel to " + targetPos);
            animator.SetTrigger(MOVE);
            characterMovement.AskForOneMove(() => {
                gridManager.myGrid[characterMovement.Position].topping = gameObject;
                Debug.Log("[Enemy] finished");
                animator.SetTrigger(STOP);
                finishedTurn.Raise();
            });
        }

        private void notAggressive() {
            animator.SetBool(AGGRESSIVE, false);
            Debug.Log("[Enemy] finished");
            finishedTurn.Raise();
        }

        private void doFlee(int turn) {
            Debug.Log("[Enemy] flee");
            gridManager.myGrid[characterMovement.Position].topping = null;
            characterMovement.Target = gridManager.myGrid[fleeTarget].transform;
            animator.SetTrigger(MOVE);
            characterMovement.AskForOneMove(
                () => {
                    Debug.Log("[Enemy] finished");
                    fleeingTurn -= 1;
                    characterMovement.Target = target.transform;
                    gridManager.myGrid[characterMovement.Position].topping = gameObject;
                    animator.SetTrigger(STOP);
                    finishedTurn.Raise();
                });
        }

        private void attack() {
            Debug.Log("[Enemy] attack");
            lifeOfMyEnemy.loseLife(attackPower);
        }

        public void flee(HexCoordinates originSound, int powerSound) {
            if (characterMovement.Position.DistanceTo(originSound) > powerSound) {
                // Do nothing, we are too far
                return;
            }

            // Flee from this position
            fleeTarget = originSound.oppositeTo(characterMovement.Position);
            fleeingTurn = maxDistanceFlee;
        }

        public void displayIsAffected(int noisePower, HexCoordinates originSound, bool enable) {
            if (characterMovement.Position.DistanceTo(originSound) <= noisePower && enable) {
                // affected
                foreach (var meshRenderer in meshRenderers) {
                    meshRenderer.material.color = colorAffected;
                }
            } else {
                foreach (var meshRenderer in meshRenderers) {
                    meshRenderer.material.color = Color.white;
                }
            }
        }
    }
}