using System.Collections;
using GameEventSystem;
using hex;
using UnityEngine;
using UnityEngine.AI;

// ReSharper disable RedundantDefaultMemberInitializer

namespace controllers {
    public class CamelController : MonoBehaviour {
        [SerializeField] private GameObject target = default;

        private NavMeshAgent navMeshAgent;
        private const double TOLERANCE = 0.01;
        [SerializeField] private GameEvent finishedTurn;
        [SerializeField] private int maxDistance;
        [SerializeField] private float animationSpeed;
        [SerializeField] private int everyNTurns = 2;
        private int currentNbTurns = 0;

        private float realMaxDistance;

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

            realMaxDistance = ExtensionsHex.realDistanceFromHexDistance(maxDistance);

            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.speed = animationSpeed;
        }

        public void DoYourTurn() {
            Debug.Log("Woohoo camel turn!");
            if (currentNbTurns >= everyNTurns) {
                currentNbTurns = 0;
                finishedTurn.Raise();
                return;
            }

            var soFar = 0f;
            currentNbTurns++;
            // Go follow your dream!
            var path = new NavMeshPath();
            navMeshAgent.CalculatePath(target.transform.position, path);
            for (var i = 0; i < path.corners.Length - 1; i++) {
                var lastSegmentVector = (path.corners[i + 1] - path.corners[i]);
                var sqrVectorDistance = lastSegmentVector.magnitude;

                if (soFar + sqrVectorDistance <= realMaxDistance) {
                    soFar += sqrVectorDistance;
                    if (i == path.corners.Length - 2) {
                        // This is the end, so go to target directly
                        navMeshAgent.SetPath(path);
                        break;
                    }
                } else {
                    // Path length exceeds maxDist
                    var finalPoint = path.corners[i] + lastSegmentVector.normalized * (maxDistance - soFar);
                    var centeredCellFinalPoint = HexCoordinates.FromPosition(finalPoint).newToPosition();
                    NavMesh.CalculatePath(transform.position, centeredCellFinalPoint, NavMesh.AllAreas, path);
                    navMeshAgent.SetPath(path);
                    break;
                }
            }

            // Start to move
            navMeshAgent.isStopped = false;
            // check for destination arrived
            StartCoroutine(sendFinishedWhenNavMeshArrives());
        }

        private IEnumerator sendFinishedWhenNavMeshArrives() {
            while (navMeshAgent.pathPending
                   || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance
                   || navMeshAgent.hasPath && navMeshAgent.velocity.sqrMagnitude > TOLERANCE
            ) {
                // The destination is not yet reached
                yield return null;
            }

            navMeshAgent.isStopped = true;
            // Reached destination
            finishedTurn.Raise();
        }
    }
}