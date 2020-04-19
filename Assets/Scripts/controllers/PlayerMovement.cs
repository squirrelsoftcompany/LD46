using System;
using System.Collections;
using hex;
using UnityEngine;

namespace controllers {
    public class PlayerMovement : MonoBehaviour {
        [SerializeField] private HexCoordinates position;

        IEnumerator moveTo(HexCoordinates toPosition) {
            throw new Exception();

            position = toPosition;
        }
    }
}