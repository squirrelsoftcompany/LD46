using hex;
using UnityEngine;

namespace controllers {
    public abstract class PositionedLivingThing : MonoBehaviour {
        public abstract HexCoordinates Position { get; }
    }
}