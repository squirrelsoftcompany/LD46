using JetBrains.Annotations;
using UnityEngine;

namespace controllers {
    public class AnimReceiver : MonoBehaviour {
        private PlayerController playerController;

        private void Awake() {
            playerController = GetComponentInParent<PlayerController>();
        }

        // Start is called before the first frame update
        void Start() { }

        [UsedImplicitly]
        // Used by animation end (noise)
        public void onFinishedAnimationSound() {
            playerController.onFinishedAnimationSound();
        }

        // Update is called once per frame
        void Update() { }
    }
}