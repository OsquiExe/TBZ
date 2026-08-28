using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps
{
    public class Spikes : BulletSpawner
    {
        [SerializeField] Vector3 playerDelta;
        Vector3 lastPlayerPosition;

        [SerializeField] float distanceBetweenSpikes = 2;
        bool spawning = false;

        private void Awake()
        {
            // PARTE 1: Escudo protector al buscar el Audio Manager
            GameObject audioObj = GameObject.FindGameObjectWithTag("Audio");
            if (audioObj != null)
            {
                AudioManager = audioObj.GetComponent<AudioManager>();
            }
        }

        private void LateUpdate()
        {
            if (transform.position != lastPlayerPosition) playerDelta = ((transform.position - lastPlayerPosition) * 100).normalized;
            else playerDelta = Vector3.up;

            lastPlayerPosition = transform.position;
        }

        protected override void OnCooldownEnds()
        {
            if (nivel - 1 < 0) return;

            StartCoroutine(spawnBullets());

            IEnumerator spawnBullets()
            {
                GameObject prefab = nivel >= 4 ? maxLevelBulletPrefabs : bulletPrefabs;

                // PARTE 2: Escudo protector al reproducir el sonido
                if (spawnClip && AudioManager != null)
                {
                    AudioManager.PlaySFX(clip);
                }

                //Cantidad
                if (spawnedBullets.Count - 1 < amountOfBullets[nivel - 1])
                {
                    int needToSpawn = amountOfBullets[nivel - 1] - spawnedBullets.Count;
                    for (int i = 0; i < needToSpawn; i++)
                    {
                        GameObject bullet = Instantiate(prefab, null);

                        yield return null;
                        Weapon w = bullet.GetComponentInChildren<Weapon>(true);

                        spawnedBulletsTriggers.Add(w);
                    }
                }

                spawning = true;
                for (int i = 0; i < spawnedBulletsTriggers.Count; i++)
                {
                    GameObject spike = spawnedBulletsTriggers[i].gameObject;

                    spike.transform.position = transform.position - playerDelta;
                    spike.SetActive(true);
                    spike.transform.parent = null;

                    spawnedBulletsTriggers[i].damage = damages[nivel - 1];

                    if (i == 0) continue;

                    while (Vector3.Distance(transform.position, spawnedBulletsTriggers[i - 1].transform.position) < distanceBetweenSpikes)
                    {
                        yield return null;

                        if (!spawning) yield break;
                    }
                }

                spawning = false;
            }
        }

        protected override void OnCooldownStarts()
        {
            spawning = false;
            base.OnCooldownStarts();
        }

        protected override void OnMaxUpgraded()
        {
            for (int i = 0; i < spawnedBulletsTriggers.Count; i++)
            {
                if (spawnedBulletsTriggers[i].gameObject != null) Destroy(spawnedBulletsTriggers[i].gameObject);
            }

            spawnedBulletsTriggers.Clear();
        }

        protected override void OnDestroy()
        {
            for (int i = 0; i < spawnedBulletsTriggers.Count; i++)
            {
                if (spawnedBulletsTriggers[i] == null) continue;

                GameObject spike = spawnedBulletsTriggers[i].gameObject;
                Destroy(spike);
            }
        }
    }
}