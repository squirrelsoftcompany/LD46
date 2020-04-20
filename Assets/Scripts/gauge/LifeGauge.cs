using GameEventSystem;
using UnityEngine;

// ReSharper disable RedundantDefaultMemberInitializer

namespace gauge {
    public class LifeGauge : MonoBehaviour {
        [SerializeField] private int life = 50;

        [SerializeField] private int maxLife = 50;
        [SerializeField] private GameEvent onDie = default;
        private Animator animator;
        private static readonly int HURT = Animator.StringToHash("hurt");

        private void Awake() {
            if (Debug.isDebugBuild && onDie == null) {
                Debug.LogWarning("onDie game event is null");
            }

            animator = GetComponentInChildren<Animator>();

            life = maxLife;
        }

        public void loseLife(int amount) {
            if (amount < 0) return;
            animator.SetTrigger(HURT);
            life -= amount;
            if (life < 0) {
                onDie.Raise();
            }
        }

        public void winLife(int amount) {
            if (amount < 0) return;
            life += amount;
        }
    }
}