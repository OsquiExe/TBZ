using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps {
    public class Boomerang : BulletSpawner {

        [Space(10)]
        [SerializeField] float speed = 80;
        [SerializeField] float boomberangRotation = 120;

        private void Awake()
        {
            AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }

        protected override void OnUpdate() {
            transform.Rotate(new Vector3(0, 0, 1), speed * Time.deltaTime, Space.Self);
            for(int i = 0; i < spawnedBullets.Count; i++) {
                spawnedBullets[i].transform.localPosition = new Vector3(2, 0, 0);
                spawnedBullets[i].transform.Rotate(new Vector3(0, 0, 1), boomberangRotation * Time.deltaTime, Space.Self);
            }
        }

        protected override void OnCooldownEnds() {
            base.OnCooldownEnds();
        }
    }

}