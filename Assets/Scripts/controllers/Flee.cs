using System.Collections.Generic;
using System.Linq;
using hex;
using UnityEngine;

namespace controllers {
    public class Flee : MonoBehaviour {
        private List<EnemyController> enemies;

        private void Start() {
            enemies = GetComponentsInChildren<EnemyController>().ToList();
        }

        public void flee(HexCoordinates originSound, int powerSound) {
            foreach (var enemy in enemies) {
                enemy.flee(originSound, powerSound);
            }
        }

        public void displayNoise(int noisePower, HexCoordinates position, bool enable) {
            foreach (var enemy in enemies) {
                enemy.displayIsAffected(noisePower, position, enable);
            }
        }
    }
}