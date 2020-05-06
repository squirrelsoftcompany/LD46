using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using hex;
using UnityEngine;
using UnityEngine.AI;

// ReSharper disable RedundantDefaultMemberInitializer

namespace controllers {
    [RequireComponent(typeof(NavMeshObstacle))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class CharacterMovement : MonoBehaviour {
        private Transform _target;
        public Transform Target { get => _target; set => _target = value; }
        /**/
        private NavMeshAgent navMeshAgent;
        private NavMeshObstacle navMeshObstacle;

        private GridManager gridManager;

        public HexCoordinates Position => transform.position.toHex();

        private bool makeOneMove = false;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshObstacle = GetComponent<NavMeshObstacle>();
            gridManager = FindObjectOfType<GridManager>();
        }

        public void Start()
        {
            Invoke("InitNavigation", 0.5f);
        }
        
        private void InitNavigation()
        {
            navMeshAgent.enabled = false;
            navMeshObstacle.enabled = true;
        }

        private IEnumerator MoveToTarget(Action onFinished)
        {
            navMeshObstacle.enabled = false;
            yield return new WaitWhile(() => navMeshObstacle.isActiveAndEnabled);

            var go = new GameObject();
            go.transform.localPosition = transform.localPosition;
            var agent = go.AddComponent<NavMeshAgent>();

            // Wait for navMeshAgent activation
            yield return new WaitWhile(() => !agent.isActiveAndEnabled);

            // calculate the path
            CalculateHexPath(agent, Target.localPosition, out var path);

            Destroy(go);

            navMeshObstacle.enabled = true;

            float delta = Time.fixedDeltaTime;
            var from = transform.localPosition;
            //var fromOrientation = transform.localRotation;
            var to = path[1].ToPosition();
            to.y = from.y;
            //var toOrientation = Quaternion.Euler(0, Vector3.Angle(Vector3.forward, to), 0);
			float speed = 2f * delta ;
			var movement = to - transform.localPosition ;
			var toOrientation = Quaternion.LookRotation(movement.normalized);
            //while (delta < 1.0f)
			while (movement.sqrMagnitude>0.1f)
            {
                //transform.localRotation = Quaternion.Lerp(fromOrientation, toOrientation, delta * 2);
                //transform.localPosition = Vector3.Lerp(from, to, delta);
				transform.rotation = Quaternion.RotateTowards(transform.rotation,toOrientation,speed*20) ;
				transform.localPosition = Vector3.MoveTowards(transform.localPosition,to,speed) ;
                yield return new WaitForFixedUpdate();
                delta += Time.fixedDeltaTime;
				movement = to - transform.localPosition ;
            }
            //transform.localRotation = toOrientation;
			//transform.LookAt(to);
            transform.localPosition = to;

            onFinished?.Invoke();
        }

        public void AskForOneMove(Action onFinished = null)
        {
            StartCoroutine(MoveToTarget(onFinished));
        }

        public bool CalculateHexPath(NavMeshAgent agent, Vector3 position, out List<HexCoordinates> hexPath)
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.SamplePosition(position, out var hit, 3.5f, NavMesh.AllAreas);
            bool result = agent.CalculatePath(hit.position, path);

            hexPath = new List<HexCoordinates>();
            if (path.corners.Length == 0)
                return result;

            hexPath.Add(path.corners[0].toHex());
            for (int i = 1; i < path.corners.Length; i++)
            {
                var to = path.corners[i];

                float distance = 0;
                do
                {
                    var from = hexPath[hexPath.Count - 1].ToPosition();

                    var direction = to - from;
                    distance = direction.magnitude;
                    direction.Normalize();
                    from = from + direction * 3.5f;

                    var hexFrom = from.toHex();
                    if (hexFrom.CompareTo(hexPath[hexPath.Count - 1]) != 0)
                        hexPath.Add(hexFrom);

                } while (distance > 3.5f);
            }
            return result;
        }
    }
}