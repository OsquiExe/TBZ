using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps {
    public class Snakes : PowerUpClass {
        [SerializeField] GameObject bulletPrefabs;

        [SerializeField] List<Rigidbody2D> spawnedBullets = new();
        [SerializeField] List<Weapon> spawnedBulletsTriggers = new();

        [SerializeField] float[] possibleRotations = { -45, 45, 225, 135 };
        [SerializeField] float bulletVelocity = 10;

        [SerializeField] int[] amountOfBullets = { 1, 2, 3, 4 };
        AudioManager AudioManager;

        private void Awake() {
            AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }

        protected override void OnCooldownEnds() {
            AudioManager.PlaySFX(AudioManager.snakesSFX);

            StartCoroutine(spawnBullets());

            IEnumerator spawnBullets() {
                if(spawnedBullets.Count - 1 < amountOfBullets[nivel - 1]) {
                    int needToSpawn = amountOfBullets[nivel - 1] - spawnedBullets.Count;
                    for(int i = 0; i < needToSpawn; i++) {
                        GameObject bullet = Instantiate(bulletPrefabs, null);
                        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

                        yield return null;
                        Weapon w = bullet.GetComponentInChildren<Weapon>(true);

                        spawnedBullets.Add(rb);
                        spawnedBulletsTriggers.Add(w);
                    }
                }

                for(int i = 0; i < spawnedBullets.Count; i++) {
                    transform.localEulerAngles = new Vector3(0, 0, possibleRotations[Random.Range(0, possibleRotations.Length)]);

                    Rigidbody2D bullet = spawnedBullets[i];

                    bullet.transform.SetPositionAndRotation(transform.position, transform.rotation);

                    bullet.gameObject.SetActive(true);
                    bullet.velocity = -bullet.transform.right * bulletVelocity;

                    //spawnedBulletsTriggers[i].damage = damages[nivel - 1];
                }
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
        }
    }

}