﻿using System;
using System.Collections;
using GameEventSystem;
using hex;
using UnityEngine;
using UnityEngine.AI;
using Grid = hex.Grid;

// ReSharper disable RedundantDefaultMemberInitializer

namespace controllers {
    public class CharacterMovement : MonoBehaviour {
        [SerializeField] private HexCoordinates position = default;
        [SerializeField] private Grid grid = default;
        private NavMeshAgent navMeshAgent;
        private const float EPSILON = 0.01f;

        public HexCoordinates Position => position;

        private void Awake() {
            position = HexCoordinates.FromPosition(transform.position);
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        IEnumerator fromPosToOther(Vector3 to, HexCoordinates toCoordinates, GameEvent finishedTurn,
            float speedAnimation, Action callback) {
            while ((to - transform.position).magnitude > EPSILON) {
                transform.position = Vector3.MoveTowards(transform.position, to, speedAnimation * Time.deltaTime);
                yield return null;
            }

            position = toCoordinates;
            if (finishedTurn) {
                finishedTurn.Raise();
            }

            callback?.Invoke();
        }

        public void navigateTo(Vector3 targetPosition, float realMaxDistance, Action onFinished) {
            var distanceSoFar = 0f;
            // Go follow your dream!
            var path = new NavMeshPath();
            navMeshAgent.CalculatePath(targetPosition, path);
            for (var i = 0; i < path.corners.Length - 1; i++) {
                var lastSegmentVector = (path.corners[i + 1] - path.corners[i]);
                var sqrVectorDistance = lastSegmentVector.magnitude;

                if (distanceSoFar + sqrVectorDistance <= realMaxDistance) {
                    distanceSoFar += sqrVectorDistance;
                    if (i == path.corners.Length - 2) {
                        // This is the end, so go to target directly
                        navMeshAgent.SetPath(path);
                        break;
                    }
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
            StartCoroutine(sendFinishedWhenNavMeshArrives(onFinished));
        }

        private IEnumerator sendFinishedWhenNavMeshArrives(Action onFinished) {
            while (navMeshAgent.pathPending
                   || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance
                   || navMeshAgent.hasPath && navMeshAgent.velocity.sqrMagnitude > EPSILON
            ) {
                // The destination is not yet reached
                yield return null;
            }

            navMeshAgent.isStopped = true;
            // Reached destination
            onFinished?.Invoke();
        }

        public void moveTo(HexCoordinates toPosition, float speedAnimation, GameEvent finishedTurn = null,
            Action callback = null) {
            var toVector3 = toPosition.ToPosition();
            toVector3.y = transform.position.y; // keep elevation
            StartCoroutine(fromPosToOther(toVector3, toPosition, finishedTurn, speedAnimation, callback));
        }
    }
}