using UnityEngine;

// ReSharper disable RedundantDefaultMemberInitializer

namespace camera {
    public class UiLookCamera : MonoBehaviour {
        [SerializeField] private Camera myCamera = default;

        private void Update() {
            var rotation = myCamera.transform.rotation;
            transform.LookAt(transform.position + rotation * Vector3.forward,
                rotation * Vector3.up);
        }
    }
}