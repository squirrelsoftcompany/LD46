using System.Collections.Generic;
using UnityEngine;

namespace TurnManager.Debug {
    public class DebugTurn : MonoBehaviour {
        [SerializeField]private List<GameObject> gameObject;
        public void printDebug(string debug) {
            UnityEngine.Debug.Log(debug);
        }

        public void sendYourTurn() {
            foreach (var game in gameObject) {
                game.SendMessage("DoYourTurn");
            }
        }
    }
}