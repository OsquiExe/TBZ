using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSpawner : MonoBehaviour {
    /*[Header("Configuraci�n del Spawner")]
    [SerializeField] private GameObject itemPrefab;

    [Tooltip("El tama�o del �rea (cuadrado) alrededor del jugador donde pueden aparecer los �tems.")]
    [SerializeField] private float spawnRadiusX = 15f;
    [SerializeField] private float spawnRadiusY = 15f;

    [Header("Tiempos de Aparici�n")]
    [SerializeField] private float minSpawnTime = 20f;
    [SerializeField] private float maxSpawnTime = 40f;

    private float spawnTimer;

    private void Start() {
        SetNextSpawnTime();
    }

    private void Update() {
        if(Time.timeScale == 0f) return;

        if(WaveManager.Instance != null && !WaveManager.Instance.isWaveActive) return;

        spawnTimer -= Time.deltaTime;

        if(spawnTimer <= 0f) {
            SpawnItem();
            SetNextSpawnTime();
        }
    }

    private void SpawnItem() {
        if(itemPrefab == null) {
            Debug.LogWarning("Falta asignar el prefab del �tem en el BombSpawner.");
            return;
        }


        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Vector3 centerPoint = player != null ? player.transform.position : transform.position;

        float randomX = Random.Range(-spawnRadiusX, spawnRadiusX);
        float randomY = Random.Range(-spawnRadiusY, spawnRadiusY);
        Vector3 spawnPosition = centerPoint + new Vector3(randomX, randomY, 0f);

        Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
    }

    private void SetNextSpawnTime() {
        spawnTimer = Random.Range(minSpawnTime, maxSpawnTime);
    }


    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Vector3 center = transform.position;

        if(Application.isPlaying) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if(player != null) {
                center = player.transform.position;
            }
        }

        Gizmos.DrawWireCube(center, new Vector3(spawnRadiusX * 2, spawnRadiusY * 2, 0));
    }*/
}