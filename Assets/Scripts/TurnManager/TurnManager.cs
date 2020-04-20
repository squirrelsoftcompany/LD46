using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Turn {
    [System.Serializable]
    public struct Part {
        public List<GameObject> objects;
    }

    public class TurnManager : MonoBehaviour {
        public List<Part> parts;
        int _partIndex = 0;
        int _turn = 0;
        int _objectTurnNotFinished = 0;
        private bool readyToMoveOn = false;
        private bool shouldRelaunchImmediately = false;

        // Start is called before the first frame update
        void Start() {
            Next();
        }

        // Update is called once per frame
        void Update() { }

        public void AddObject(GameObject go, int part) {
            Debug.Assert(part >= 0 && part < parts.Count);
            parts[part].objects.Add(go);
        }

        public void OneObjectFinishedItsTurnSlot() {
            _objectTurnNotFinished--;
            if (!readyToMoveOn && _objectTurnNotFinished <= 0) {
                shouldRelaunchImmediately = true;
                return;
            }

            if (_objectTurnNotFinished <= 0) {
                shouldRelaunchImmediately = false;
                Next();
            }
        }

        public void Next() {
            readyToMoveOn = false;
            _objectTurnNotFinished = 0;
            foreach (GameObject go in parts[_partIndex].objects) {
                _objectTurnNotFinished++;
                go.SendMessage("DoYourTurn");
            }

            _partIndex = (_partIndex + 1) % parts.Count;
            if (_partIndex == 0)
                _turn++;
            readyToMoveOn = true;
            if (shouldRelaunchImmediately) {
                shouldRelaunchImmediately = false;
                Next();
            }
        }
    }
}