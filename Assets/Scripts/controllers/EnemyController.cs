using GameEventSystem;
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
        private CharacterMovement characterMovement;
        private NavMeshAgent navMeshAgent;
        private LifeGauge lifeOfMyEnemy;
        [SerializeField] private int attackPower = 1;
        private bool fleeing;
        private HexCoordinates fleeTarget;
        private MeshRenderer[] meshRenderers;

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
        }

        // Method called by TurnManager, do not rename
        [UsedImplicitly]
        public void DoYourTurn() {
            Debug.Log("[Enemy] my turn");

            // if fleeing, then flee, and stop fleeing
            if (fleeing) {
                doFlee();
                return;
            }

            // find position to go to
            var targetPos = target.transform.position;
            // if within range of attack, then attack
            var hexTarget = HexCoordinates.FromPosition(targetPos);
            if (hexTarget.DistanceTo(characterMovement.Position) <= attackRange) {
                attack();
                Debug.Log("[Enemy] finished");
                finishedTurn.Raise();
                return;
            }

            targetPos.y = transform.position.y;

            Debug.Log("[Enemy] travel to " + targetPos);
            characterMovement.navigateTo(
                targetPosition: targetPos,
                realMaxDistance: maxDistanceTravel.realDistanceFromHexDistance(),
                stoppingDistanceTarget: STOPPING_DISTANCE_TARGET,
                onFinished: () => {
                    Debug.Log("[Enemy] finished");
                    finishedTurn.Raise();
                });
        }

        private void doFlee() {
            Debug.Log("[Enemy] flee");
            characterMovement.navigateTo(
                fleeTarget.ToPosition(), maxDistanceFlee.realDistanceFromHexDistance(),
                STOPPING_DISTANCE_TARGET, () => {
                    Debug.Log("[Enemy] finished");
                    // Only flee for one turn
                    fleeing = false;
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
            fleeing = true;
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