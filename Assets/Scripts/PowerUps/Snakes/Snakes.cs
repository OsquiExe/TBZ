using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps {
    public class Snakes : BulletSpawner {
       /* [SerializeField] GameObject bulletPrefabs;

        [SerializeField] List<Rigidbody2D> spawnedBullets = new();
        [SerializeField] List<Weapon> spawnedBulletsTriggers = new();

        [SerializeField] float[] possibleRotations = { -45, 45, 225, 135 };
        [SerializeField] float bulletVelocity = 10;

        [SerializeField] int[] amountOfBullets = { 1, 2, 3, 4 };
        AudioManager AudioManager;*/

        private void Awake() {
            AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }
    }

}