using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps {
    public class Missil : PowerUpClass {
        [SerializeField] Rigidbody2D bullet;
        [SerializeField] float rotation = -45;
        [SerializeField] float bulletVelocity = 10;

        private void Awake() {
            bullet.transform.parent = null;
        }

        protected override void OnCooldownEnds() {
            transform.localEulerAngles = new Vector3(0, 0, rotation);
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
    }

}