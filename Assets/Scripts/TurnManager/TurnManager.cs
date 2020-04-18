using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Turn
{
    [System.Serializable]
    public struct Part
    {
        public List<GameObject> objects;
    }

    public class TurnManager : MonoBehaviour
    {
        public List<Part> parts;
        int _partIndex = 0;
        int _turn = 0;
        int _objectTurnNotFinished = 0;

        // Start is called before the first frame update
        void Start()
        {
            Next();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void AddObject(GameObject go, int part)
        {
            Debug.Assert(part >= 0 && part < parts.Count);
            parts[part].objects.Add(go);
        }

        public void OneObjectFinishedItsTurnSlot()
        {
            _objectTurnNotFinished--;
            if (_objectTurnNotFinished <= 0)
            {
                Next();
            }
        }

        public void Next()
        {
            _objectTurnNotFinished = 0;
            foreach (GameObject go in parts[_partIndex].objects)
            {
                go.SendMessage("DoYourTurn");
                _objectTurnNotFinished++;
            }

            _partIndex = (_partIndex + 1) % parts.Count;
            if (_partIndex == 0)
                _turn++;
        }
    }

}