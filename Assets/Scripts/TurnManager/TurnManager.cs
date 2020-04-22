using System;
using System.Collections.Generic;
using UnityEngine;

namespace Turn {
    [Serializable]
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
            _objectTurnNotFinished = parts[_partIndex].objects.Count;

            foreach (GameObject go in parts[_partIndex].objects) {
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

        public void pause(bool pause) {
            Time.timeScale = pause ? 0f : 1f;
        }

        // public functions to fill the parts

        public void AddPlayer(GameObject go)
        {
            AddObject(go, 0);
        }

        public void AddCamel(GameObject go)
        {
            AddObject(go, 1);
        }

        private int nbWolf = 0;
        public void AddWolf(GameObject go)
        {
            AddObject(go, 2+nbWolf);
            nbWolf++;
        }

        private void AddObject(GameObject go, int part)
        {
            Debug.Assert(part >= 0);
            if (part > parts.Count - 1)
            {
                AddPart();
                parts[parts.Count - 1].objects.Add(go);
            }
            else
                parts[part].objects.Add(go);
        }

        private void AddPart()
        {
            var part = new Part();
            part.objects = new List<GameObject>();
            parts.Add(part);
        }
    }
}