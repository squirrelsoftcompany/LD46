using System.Collections.Generic;
using UnityEngine;

namespace TurnManager.Debug {
    // ReSharper disable RedundantDefaultMemberInitializer

    public class DebugTurn : MonoBehaviour {
        [SerializeField] private List<GameObject> gameObjects = default;

        public void printDebug(string debug) {
            UnityEngine.Debug.Log(debug);
        }

        public void sendYourTurn() {
            foreach (var game in gameObjects) {
                game.SendMessage("DoYourTurn");
            }
        }
    }
}