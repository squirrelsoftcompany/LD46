using System;
using System.Collections;
using GameEventSystem;
using hex;
using UnityEngine;
using Grid = hex.Grid;

// ReSharper disable RedundantDefaultMemberInitializer

namespace controllers {
    public class CharacterMovement : MonoBehaviour {
        [SerializeField] private float speedAnimation = 4;
        [SerializeField] private HexCoordinates position = default;
        [SerializeField] private Grid grid = default;
        private const float EPSILON = 0.01f;

        public HexCoordinates Position => position;

        private void Awake() {
            position = HexCoordinates.FromPosition(transform.position);
        }

        IEnumerator fromPosToOther(Vector3 to, HexCoordinates toCoordinates, GameEvent finishedTurn, Action callback) {
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


        public void moveTo(HexCoordinates toPosition, GameEvent finishedTurn = null, Action callback = null) {
            var toVector3 = toPosition.ToPosition();
            toVector3.y = transform.position.y; // keep elevation
            StartCoroutine(fromPosToOther(toVector3, toPosition, finishedTurn, callback));
        }
    }
}