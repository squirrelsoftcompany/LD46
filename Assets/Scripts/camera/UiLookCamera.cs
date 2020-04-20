using System;
using UnityEngine;

// ReSharper disable RedundantDefaultMemberInitializer

namespace camera {
    public class UiLookCamera : MonoBehaviour {
        [SerializeField] private Camera myCamera = default;

        private void Awake() {
            if (myCamera == null) {
                myCamera = Camera.main;
            }
        }

        private void Update() {
            if (!isActiveAndEnabled) return;
            var rotation = myCamera.transform.rotation;
            transform.LookAt(transform.position + rotation * Vector3.forward,
                rotation * Vector3.up);
        }
    }
}