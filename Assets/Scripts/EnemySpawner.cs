using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject spawnee;
    [SerializeField] float sizeX = 1f;
    [SerializeField] float sizeY = 1f;
    [SerializeField] float spawnCooldown = 1f;

    private float spawnTime; 

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = spawnCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnTime > 0)
        {
            spawnTime -= Time.deltaTime;
        }

        if (spawnTime <= 0)
        {
            Spawn();
            spawnTime = spawnCooldown;
        }
    }

    void Spawn()
    {
        float xPos = Random.Range(-sizeX, sizeX) + transform.position.x;
        float yPos = Random.Range(-sizeY, sizeY) + transform.position.y;

        Vector3 spawnPosition = new Vector3(xPos, yPos,0);
        Instantiate(spawnee, spawnPosition, Quaternion.identity);
    }
}
