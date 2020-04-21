using System;
using System.Collections;
using hex;
using UnityEngine;
using UnityEngine.AI;

// ReSharper disable RedundantDefaultMemberInitializer

namespace controllers {
    public class CharacterMovement : MonoBehaviour {
        private NavMeshAgent navMeshAgent;
        private const float EPSILON = 0.01f;
        private static readonly float STOPPING_DISTANCE_PRECISE = 1.realDistanceFromHexDistance() / 5;

        public HexCoordinates Position => transform.position.toHex();

        private void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        IEnumerator fromPosToOther(Vector3 to, HexCoordinates toCoordinates, Action onFinishedAction,
            float speedAnimation) {
            while ((to - transform.position).magnitude > EPSILON) {
                transform.position = Vector3.MoveTowards(transform.position, to, speedAnimation * Time.deltaTime);
                yield return null;
            }

            onFinishedAction?.Invoke();
        }

        public void navigateTo(Vector3 targetPosition, float realMaxDistance, float stoppingDistanceTarget,
            Action onFinished) {
            var distanceSoFar = 0f;
            // Go follow your dream!
            var path = new NavMeshPath();
            navMeshAgent.stoppingDistance = stoppingDistanceTarget;
            navMeshAgent.CalculatePath(targetPosition, path);
            for (var i = 0; i < path.corners.Length - 1; i++) {
                var lastSegmentVector = (path.corners[i + 1] - path.corners[i]);
                var vectorDistance = lastSegmentVector.magnitude;

                if (distanceSoFar + vectorDistance <= realMaxDistance) {
                    distanceSoFar += vectorDistance;
                    if (i != path.corners.Length - 2) continue;
                    // This is the end, so go to target directly
                    navMeshAgent.SetPath(path);
                    break;
                } else {
                    // Path length exceeds maxDist
                    var finalPoint = path.corners[i] + lastSegmentVector.normalized * (realMaxDistance - distanceSoFar);
                    var centeredCellFinalPoint = HexCoordinates.FromPosition(finalPoint).ToPosition();
                    NavMesh.CalculatePath(transform.position, centeredCellFinalPoint, NavMesh.AllAreas, path);
                    navMeshAgent.SetPath(path);
                    break;
                }
            }

            // Start to move
            navMeshAgent.isStopped = false;
            // check for destination arrived
            StartCoroutine(centerPositionInCell(onFinished));
        }

        private IEnumerator centerPositionInCell(Action onFinished) {
            while (stillRunning()) {
                yield return null;
            }

            var position = HexCoordinates.FromPosition(transform.position);
            var centerCell = position.ToPosition();
            if ((transform.position - centerCell).sqrMagnitude > EPSILON) {
                navMeshAgent.stoppingDistance = STOPPING_DISTANCE_PRECISE;
                navMeshAgent.SetDestination(position.ToPosition()); // center in cell
                StartCoroutine(sendFinishedWhenNavMeshArrives(onFinished));
            } else {
                navMeshAgent.isStopped = true;
                onFinished?.Invoke();
            }
        }

        private bool stillRunning() {
            return navMeshAgent.pathPending
                   || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance
                   || navMeshAgent.hasPath && navMeshAgent.velocity.sqrMagnitude > EPSILON;
        }

        private IEnumerator sendFinishedWhenNavMeshArrives(Action onFinished) {
            while (stillRunning()) {
                // The destination is not yet reached
                yield return null;
            }

            navMeshAgent.isStopped = true;
            // Reached destination
            onFinished?.Invoke();
        }

        public void moveTo(HexCoordinates toPosition, float speedAnimation, Action onFinishedAction = null) {
            var toVector3 = toPosition.ToPosition();
            toVector3.y = transform.position.y; // keep elevation
            // transform.LookAt(toVector3);
            StartCoroutine(fromPosToOther(toVector3, toPosition, onFinishedAction, speedAnimation));
        }
    }
}