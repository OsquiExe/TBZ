using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps {
    public class Snakes : PowerUpClass {
        [SerializeField] Rigidbody2D bullet;
        [SerializeField] float[] possibleRotations = { -45, 45, 225, 135 };
        [SerializeField] float bulletVelocity = 10;

        private void Awake() {
            bullet.transform.parent = null;
        }

        protected override void OnCooldownEnds() {
            transform.localEulerAngles = new Vector3(0, 0, possibleRotations[Random.Range(0, possibleRotations.Length)]);
            bullet.transform.SetPositionAndRotation(transform.position, transform.rotation);

            bullet.gameObject.SetActive(true);
            bullet.velocity = -bullet.transform.right * bulletVelocity;
        }

        protected override void OnCooldownStarts() {
            bullet.gameObject.SetActive(false);
        }

        protected override void OnUpdate() {
            base.OnUpdate();
        }

        private void OnDestroy() {
            if(bullet != null && bullet.gameObject != null) Destroy(bullet.gameObject);
        }
    }

}