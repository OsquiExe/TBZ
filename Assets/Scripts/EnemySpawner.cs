using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject spawnee;
    [SerializeField] private float sizeX = 1f;
    [SerializeField] private float sizeY = 1f;

    [SerializeField] private float baseSpawnCooldown = 1f;

    private float spawnTime;

    void Start()
    {
        spawnTime = baseSpawnCooldown;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

  
        if (WaveManager.Instance != null && !WaveManager.Instance.CanSpawn()) return;

        if (spawnTime > 0)
        {
            spawnTime -= Time.deltaTime;
        }

        if (spawnTime <= 0)
        {
            Spawn();


            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnEnemySpawned();
                float waveMultiplier = WaveManager.Instance.GetSpawnMultiplier();
                spawnTime = baseSpawnCooldown * waveMultiplier;
            }
            else
            {
                spawnTime = baseSpawnCooldown;
            }
        }
    }

    void Spawn()
    {
        if (spawnee == null)
        {
            Debug.LogError($"El prefab 'spawnee' no está asignado o fue destruido en el Spawner: {gameObject.name}", gameObject);
            return;
        }

        float xPos = Random.Range(-sizeX, sizeX) + transform.position.x;
        float yPos = Random.Range(-sizeY, sizeY) + transform.position.y;

        Vector3 spawnPosition = new Vector3(xPos, yPos, 0);
        Instantiate(spawnee, spawnPosition, Quaternion.identity);
    }
}