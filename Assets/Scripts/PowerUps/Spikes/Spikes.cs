using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PowerUps {
    public class Spikes : PowerUpClass {
        [SerializeField] GameObject spikePrefab;
        [SerializeField] Vector3 playerDelta;
        Vector3 lastPlayerPosition;

        [SerializeField] List<Weapon> spawnedSpikesTriggers = new();
        [SerializeField] float distanceBetweenSpikes = 2;
        [SerializeField] int[] amountOfSpikes = { 1, 3, 4, 8 };
        [SerializeField] int[] damages = { 2, 4, 6, 8 };
        bool spawning = false;

        AudioManager AudioManager;

        private void Awake()
        {
            AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }

        private void LateUpdate() {
            if(transform.position != lastPlayerPosition) playerDelta = ((transform.position - lastPlayerPosition) * 100).normalized;
            else playerDelta = Vector3.up;

            lastPlayerPosition = transform.position;
        }

        protected override void OnCooldownEnds() {
            if(nivel - 1 < 0) return;

            StartCoroutine(spawnBullets());

            IEnumerator spawnBullets() {
                if(spawnedSpikesTriggers.Count - 1 < amountOfSpikes[nivel - 1]) {
                    int needToSpawn = amountOfSpikes[nivel - 1] - spawnedSpikesTriggers.Count;

                    for(int i = 0; i < needToSpawn; i++) {
                        GameObject bullet = Instantiate(spikePrefab, null);

                        yield return null;
                        Weapon w = bullet.GetComponentInChildren<Weapon>(true);
                        spawnedSpikesTriggers.Add(w);
                    }
                    AudioManager.PlaySFX(AudioManager.spikesSFX);
                }

                spawning = true;
                for(int i = 0; i < spawnedSpikesTriggers.Count; i++) {
                    GameObject spike = spawnedSpikesTriggers[i].gameObject;

                    spike.transform.position = transform.position - playerDelta;
                    spike.SetActive(true);
                    spike.transform.parent = null;

                    spawnedSpikesTriggers[i].damage = damages[nivel - 1];

                    if(i == 0) continue;

                    while(Vector3.Distance(transform.position, spawnedSpikesTriggers[i - 1].transform.position) < distanceBetweenSpikes) {
                        yield return null;

                        if(!spawning) yield break;
                    }
                }

                spawning = false;
            }
        }

        protected override void OnCooldownStarts() {
            spawning = false;

            for(int i = 0; i < spawnedSpikesTriggers.Count; i++) {
                GameObject spike = spawnedSpikesTriggers[i].gameObject;
                spike.SetActive(false);
            }
        }

        private void OnDestroy() {
            for(int i = 0; i < spawnedSpikesTriggers.Count; i++) {
                if(spawnedSpikesTriggers[i] == null) continue;

                GameObject spike = spawnedSpikesTriggers[i].gameObject;
                Destroy(spike);
            }
        }
    }
}