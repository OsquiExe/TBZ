using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : PowerUpClass {
    [SerializeField] protected GameObject bulletPrefabs;
    [SerializeField] protected GameObject maxLevelBulletPrefabs;

    [Space(10)]
    [SerializeField] protected List<Rigidbody2D> spawnedBullets = new();
    [SerializeField] protected List<Weapon> spawnedBulletsTriggers = new();

    [Space(10)]

    Vector2 originalSize;
    [SerializeField] protected bool spawnClip = true;
    [SerializeField] protected AudioManager.AudioClipNames clip;

    [Space(10)]
    [SerializeField] protected bool bulletMovesWithPlayer = false;
    protected AudioManager AudioManager;

    //-Cantidad
    [SerializeField] protected int[] amountOfBullets = { 1, 3, 4, 8 };
    [SerializeField] protected float[] possibleRotations = { -45, 45, 225, 135 };
    //-Daño
    [SerializeField] protected int[] damages = { 2, 4, 6, 8 };
    //-Tamaño
    [SerializeField] protected float[] sizes;
    //-Velocidad
    [SerializeField] protected float[] bulletVelocities = { 10, 15, 20, 25 };

    private void Awake() {
        AudioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    protected override void OnCooldownEnds() {
        if(nivel - 1 < 0) return;

        StartCoroutine(spawnBullets());

        if(spawnClip) AudioManager.PlaySFX(clip);

        IEnumerator spawnBullets() {
            GameObject prefab = nivel >= 4 ? maxLevelBulletPrefabs : bulletPrefabs;
            originalSize = prefab.transform.localScale;

            //Cantidad
            if(spawnedBullets.Count - 1 < amountOfBullets[nivel - 1]) {
                int needToSpawn = amountOfBullets[nivel - 1] - spawnedBullets.Count;
                for(int i = 0; i < needToSpawn; i++) {
                    GameObject bullet = Instantiate(prefab, null);
                    Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

                    yield return null;
                    Weapon w = bullet.GetComponentInChildren<Weapon>(true);

                    spawnedBullets.Add(rb);
                    spawnedBulletsTriggers.Add(w);
                }
            }

            //Velocidad
            for(int i = 0; i < spawnedBullets.Count; i++) {
                transform.localEulerAngles = new Vector3(0, 0, possibleRotations[Random.Range(0, possibleRotations.Length)]);

                Rigidbody2D bullet = spawnedBullets[i];

                bullet.transform.SetPositionAndRotation(transform.position, transform.rotation);

                bullet.gameObject.SetActive(true);
                bullet.velocity = -bullet.transform.right * bulletVelocities[nivel - 1];

                //Daño
                spawnedBulletsTriggers[i].damage = damages[nivel - 1];
            }

            //Tamaño
            UpdateSizeUpdateSize();
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

    protected override void OnMaxUpgraded() {
        for(int i = 0; i < spawnedBullets.Count; i++) {
            if(spawnedBullets[i].gameObject != null) Destroy(spawnedBullets[i].gameObject);
        }

        spawnedBullets.Clear();
    }

    void UpdateSizeUpdateSize() {
        if(sizes.Length == 0) return;
        StartCoroutine(corrutine());

        IEnumerator corrutine() {

            Transform originalParent = bulletMovesWithPlayer ? transform : null;

            for(int i = 0; i < spawnedBullets.Count; i++) {
                spawnedBullets[i].transform.parent = null;
            }

            yield return null;

            for(int i = 0; i < spawnedBullets.Count; i++) {
                spawnedBullets[i].transform.localScale = originalSize * sizes[nivel - 1];
                spawnedBullets[i].transform.parent = originalParent;
            }
        }
    }

    protected virtual void OnDestroy() {
        for(int i = 0; i < spawnedBullets.Count; i++) {
            Rigidbody2D bullet = spawnedBullets[i];
            if(bullet != null && bullet.gameObject != null) Destroy(bullet.gameObject);
        }
    }
}