using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps {
    public class Fireball : BulletSpawner {
        /*[SerializeField] GameObject bulletPrefabs;
        [SerializeField] List<Rigidbody2D> spawnedBullets = new();
        [SerializeField] List<Weapon> spawnedBulletsTriggers = new();

        [Space(10)]
        [SerializeField] float bulletVelocity = 10;

        [Space(10)]
        [SerializeField] float[] possibleRotations = { -45, 45, 225, 135 };
        [SerializeField] int[] amountOfBullets = { 1, 2, 3, 4 };
        [SerializeField] int[] damages = { 2, 4, 6, 8 };
        [SerializeField] float[] sizes;
        Vector2 originalSize;

        AudioManager AudioManager;

        private void Awake() {
            originalSize = bulletPrefabs.transform.localScale;
            AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }

        protected override void OnCooldownEnds() {
            if(nivel - 1 < 0) return;

            if(spawnedBullets.Count - 1 < amountOfBullets[nivel - 1]) {
                int needToSpawn = amountOfBullets[nivel - 1] - spawnedBullets.Count;
                for(int i = 0; i < needToSpawn; i++) {
                    GameObject bullet = Instantiate(bulletPrefabs, null);
                    spawnedBullets.Add(bullet.GetComponent<Rigidbody2D>());
                    spawnedBulletsTriggers.Add(bullet.GetComponent<Weapon>());
                }
                AudioManager.PlaySFX(AudioManager.fireBallSFX);
            }

            for(int i = 0; i < spawnedBullets.Count; i++) {
                transform.localEulerAngles = new Vector3(0, 0, possibleRotations[Random.Range(0, possibleRotations.Length)]);

                Rigidbody2D bullet = spawnedBullets[i];

                bullet.transform.SetPositionAndRotation(transform.position, transform.rotation);

                bullet.gameObject.SetActive(true);
                bullet.velocity = -bullet.transform.right * bulletVelocity;
                bullet.transform.localScale = originalSize * sizes[nivel - 1];

                spawnedBulletsTriggers[i].damage = damages[nivel - 1];
            }
        }

        protected override void OnCooldownStarts() {
            for(int i = 0; i < spawnedBullets.Count; i++) {
                Rigidbody2D bullet = spawnedBullets[i];
                bullet.gameObject.SetActive(false);
            }
        }

        protected override void OnUpdate() {
            base.OnUpdate();
        }

        private void OnDestroy() {
            for(int i = 0; i < spawnedBullets.Count; i++) {
                Rigidbody2D bullet = spawnedBullets[i];
                if(bullet != null && bullet.gameObject != null) Destroy(bullet.gameObject);
            }
        }*/
    }

}