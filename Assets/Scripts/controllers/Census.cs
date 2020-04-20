using System.Collections.Generic;
using System.Linq;
using hex;
using UnityEngine;

namespace controllers {
    public class Census : MonoBehaviour {
        private List<CharacterMovement> movingThings;

        // Start is called before the first frame update
        private void Start() {
            movingThings = GetComponentsInChildren<CharacterMovement>().ToList();
        }

        public bool isOccupiedCell(HexCoordinates cellCoordinates) {
            return movingThings.Any(movement => movement.Position.Equals(cellCoordinates));
        }
    }
}