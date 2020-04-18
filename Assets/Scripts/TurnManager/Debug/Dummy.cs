using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Turn
{

    public class Dummy : MonoBehaviour
    {
        string messageStart = "Doing its turn...";
        string messageStop = "Done.";
        public GameEventSystem.GameEvent thingDoneEvent;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DoYourTurn()
        {
            StartCoroutine(MyCoroutine());

            Debug.Log(ToString() + " " + messageStart);
        }

        IEnumerator MyCoroutine()
        {
            yield return new WaitForSeconds(5);

            Debug.Log(ToString() + " " + messageStop);
            thingDoneEvent.Raise();
        }
    }

}