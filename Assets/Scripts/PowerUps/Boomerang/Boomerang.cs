using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps {
    public class Boomerang : PowerUpClass {

        [Space(10)]
        [SerializeField] float speed = 80;
        [SerializeField] float boomberangRotation = 120;
        [SerializeField] SpriteRenderer renderer;
        [SerializeField] Collider2D dmgScript;

        [SerializeField] Weapon triggerDmg;

        [SerializeField] float[] sizes;
        [SerializeField] int[] damages = { 2, 4, 6, 8 };

        AudioManager AudioManager;

        private void Awake()
        {
            AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }

        protected override void OnUpdate() {
            transform.Rotate(new Vector3(0, 0, 1), speed * Time.deltaTime, Space.Self);
            triggerDmg.transform.Rotate(new Vector3(0, 0, 1), boomberangRotation * Time.deltaTime, Space.Self);
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

            triggerDmg.damage = damages[nivel - 1];
            dmgScript.transform.localScale = Vector3.one * sizes[nivel - 1];
            AudioManager.PlaySFX(AudioManager.boomerangSFX);
        }
    }

}