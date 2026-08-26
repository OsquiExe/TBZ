using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PowerUps {
    public class Laser : BulletSpawner {

        //[SerializeField] float[] bulletVelocities = { 10, 15, 20, 25 };
        /*
        protected override void OnCooldownEnds() {
            if(nivel - 1 < 0) return;

            if(spawnedBullets.Count - 1 < amountOfBullets[nivel - 1]) {
                int needToSpawn = amountOfBullets[nivel - 1] - spawnedBullets.Count;
                for(int i = 0; i < needToSpawn; i++) {
                    GameObject bullet = Instantiate(bulletPrefabs, null);
                    spawnedBullets.Add(bullet.GetComponent<Rigidbody2D>());
                    spawnedBulletsTriggers.Add(bullet.GetComponent<Weapon>());
                }
            }

            for(int i = 0; i < spawnedBullets.Count; i++) {
                transform.localEulerAngles = new Vector3(0, 0, possibleRotations[Random.Range(0, possibleRotations.Length)]);

                Rigidbody2D bullet = spawnedBullets[i];

                bullet.transform.SetPositionAndRotation(transform.position, transform.rotation);

                bullet.gameObject.SetActive(true);
                bullet.velocity = -bullet.transform.right * bulletVelocities[nivel - 1];

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