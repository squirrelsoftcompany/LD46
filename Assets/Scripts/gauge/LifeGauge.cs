using GameEventSystem;
using UnityEngine;

namespace gauge {
    public class LifeGauge : MonoBehaviour {
        [SerializeField] private int life;

        [SerializeField] private int maxLife;
        [SerializeField] private GameEvent onDie;

        private void Awake() {
            if (Debug.isDebugBuild && onDie == null) {
                Debug.LogWarning("onDie game event is null");
            }

            life = maxLife;
        }

        public void loseLife(int amount) {
            if (amount < 0) return;
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