using System;
using UnityEngine;

namespace camera {
    public class UILookCamera : MonoBehaviour {
        [SerializeField] private Camera camera;

        private void Update() {
            transform.LookAt(transform.position + camera.transform.rotation * Vector3.forward,
                camera.transform.rotation * Vector3.up);
        }
    }
}