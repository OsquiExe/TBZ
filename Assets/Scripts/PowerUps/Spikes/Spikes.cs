using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps {
    public class Spikes : PowerUpClass {
        [SerializeField] GameObject spike;
        [SerializeField] Vector3 playerDelta;
        Vector3 lastPlayerPosition;

        private void LateUpdate() {
            if(transform.position != lastPlayerPosition) playerDelta = ((transform.position - lastPlayerPosition) * 100).normalized;
            else playerDelta = Vector3.up;

            lastPlayerPosition = transform.position;
        }

        protected override void OnCooldownEnds() {
            spike.transform.position = transform.position - playerDelta;
            spike.SetActive(true);
            spike.transform.parent = null;
        }

        protected override void OnCooldownStarts() {
            spike.SetActive(false);
        }

        private void OnDestroy() {
            Destroy(spike);
        }
    }
}