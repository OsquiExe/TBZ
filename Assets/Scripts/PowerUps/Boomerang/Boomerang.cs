using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps {
    public class Boomerang : PowerUpClass {

        [Space(10)]
        [SerializeField] float speed = 80;
        [SerializeField] SpriteRenderer renderer;
        [SerializeField] Collider2D dmgScript;

        protected override void OnUpdate() {
            transform.Rotate(new Vector3(0, 0, 1), speed * Time.deltaTime, Space.Self);
        }

        protected override void OnCooldownStarts() {
            Color c = renderer.color;
            c.a = 0f;

            renderer.color = c;
            dmgScript.enabled = false;
        }

        protected override void OnCooldownEnds() {
            Color c = renderer.color;
            c.a = 1f;

            renderer.color = c;
            dmgScript.enabled = true;
        }
    }

}