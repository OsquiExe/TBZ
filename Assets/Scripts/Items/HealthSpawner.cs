using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject healthPrefab; // Arrastrá acá tu prefab de la cura

    [Header("Configuración de Aparición")]
    [SerializeField] private float spawnInterval = 25f; 
    [SerializeField] private float spawnRadiusX = 15f;  
    [SerializeField] private float spawnRadiusY = 15f; 

    private float timer;

    private void Start()
    {

        timer = spawnInterval;
    }

    private void Update()
    {
      
        if (Time.timeScale == 0f) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnHealthItem();
         
            timer = spawnInterval;
        }
    }

    private void SpawnHealthItem()
    {
        if (healthPrefab == null)
        {
            Debug.LogWarning("⚠️ No asignaste el prefab de la cura en el HealthSpawner.");
            return;
        }


        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 centerPoint = player != null ? player.transform.position : transform.position;

        float randomX = Random.Range(-spawnRadiusX, spawnRadiusX);
        float randomY = Random.Range(-spawnRadiusY, spawnRadiusY);

        Vector3 spawnPos = centerPoint + new Vector3(randomX, randomY, 0f);

        // Creamos la cura
        Instantiate(healthPrefab, spawnPos, Quaternion.identity);
    }
}