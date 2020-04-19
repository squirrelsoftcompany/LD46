using UnityEngine;
// ReSharper disable RedundantDefaultMemberInitializer

namespace camera {
    public class UILookCamera : MonoBehaviour {
        [SerializeField] private Camera myCamera = default;

        private void Update() {
            transform.LookAt(transform.position + myCamera.transform.rotation * Vector3.forward,
                myCamera.transform.rotation * Vector3.up);
        }
    }
}