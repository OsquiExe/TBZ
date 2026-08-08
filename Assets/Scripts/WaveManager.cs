using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("UI de Oleada")]
    [SerializeField] private Image waveProgressFill;
    [SerializeField] private TextMeshProUGUI waveText;

    [Header("Progreso y Estado")]
    public int currentWave = 1;
    public bool isWaveActive = false;
    private int enemiesSpawnedThisWave = 0;
    private int enemiesKilledThisWave = 0;
    private int totalEnemiesThisWave = 10;

    [Header("Escalado de Dificultad")]
    [SerializeField] private int baseEnemies = 10;
    [SerializeField] private int enemiesIncreasePerWave = 5;
    [SerializeField] private float spawnCooldownMultiplier = 1f;
    [SerializeField] private float timeBetweenWaves = 3f;
    public int bossWaveIndex = 18; 

    [Header("Mega Moneda (Cada 6 Rondas)")]
    [SerializeField] private GameObject megaCoinPrefab;
    [SerializeField] private float spawnRadiusX = 15f;
    [SerializeField] private float spawnRadiusY = 15f;

    [Header("Jefe Final")] 
    [SerializeField] private GameObject bossPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        StartWave();
    }

    private void StartWave()
    {
        enemiesSpawnedThisWave = 0;
        enemiesKilledThisWave = 0;

        int powerUpCount = PowerUpManager.Instance != null ? PowerUpManager.Instance.GetSelectedPowerUpsCount() : 0;
        spawnCooldownMultiplier = Mathf.Max(0.2f, 1f - (currentWave * 0.05f) - (powerUpCount * 0.05f));

        if (currentWave == bossWaveIndex)
        {
            Debug.Log("🚨 ¡ALERTA! OLEADA DEL JEFE FINAL 🚨");


            totalEnemiesThisWave = 1;
            isWaveActive = true;
            UpdateUI();

            SpawnBoss();
        }
        else
        {
            totalEnemiesThisWave = baseEnemies + ((currentWave - 1) * enemiesIncreasePerWave) + (powerUpCount * 3);
            isWaveActive = true;
            UpdateUI();

            Debug.Log($"🌊 INICIA OLEADA {currentWave} | Enemigos totales: {totalEnemiesThisWave} | Vel. Spawn: {spawnCooldownMultiplier}x");

            if (currentWave % 6 == 0 && megaCoinPrefab != null)
            {
                SpawnMegaCoin();
            }
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("No hay Boss Prefab asignado en el WaveManager.");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 centerPoint = player != null ? player.transform.position : transform.position;

        Vector3 spawnPosition = centerPoint + new Vector3(0, spawnRadiusY, 0f);

        GameObject bossObj = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);


        JefeController bossScript = bossObj.GetComponent<JefeController>();
        if (bossScript != null)
        {
            bossScript.vidaMaxima = 1500f; 
        }

        bossObj.transform.localScale = new Vector3(3f, 3f, 3f);

        enemiesSpawnedThisWave++;
        Debug.Log("💀 ¡EL JEFE HA APARECIDO!");
    }

    public bool CanSpawn()
    {
        return isWaveActive && (enemiesSpawnedThisWave < totalEnemiesThisWave);
    }

    public void OnEnemySpawned()
    {
        enemiesSpawnedThisWave++;
    }

    public void OnEnemyKilled()
    {
        enemiesKilledThisWave++;
        UpdateUI();

        if (enemiesKilledThisWave >= totalEnemiesThisWave && isWaveActive)
        {
            EndWave();
        }
    }

    private void EndWave()
    {
        isWaveActive = false;

        if (currentWave >= bossWaveIndex)
        {
            Debug.Log("👑 ¡Jefe derrotado! Victoria.");
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerVictory();
            }
        }
        else
        {
            Debug.Log($" Oleada {currentWave} Superada. Tomando un descanso...");
            if (waveText != null) waveText.text = "¡Oleada Superada!";
            StartCoroutine(WaveTransitionRoutine());
        }
    }

    private IEnumerator WaveTransitionRoutine()
    {
        yield return new WaitForSeconds(timeBetweenWaves);
        currentWave++;
        StartWave();
    }

    private void UpdateUI()
    {
        if (waveProgressFill != null)
        {
            waveProgressFill.fillAmount = (float)enemiesKilledThisWave / totalEnemiesThisWave;
        }
        if (waveText != null && isWaveActive)
        {
            waveText.text = "Oleada " + currentWave;
        }
    }

    public float GetSpawnMultiplier() { return spawnCooldownMultiplier; }
    public float GetHealthMultiplier() { return 1f + (currentWave * 0.15f); }
    public float GetDamageMultiplier() { return 1f + (currentWave * 0.10f); }
    public float GetSpeedMultiplier() { return 1f + (currentWave * 0.02f); }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = transform.position;
        if (Application.isPlaying)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) center = player.transform.position;
        }
        Gizmos.DrawWireCube(center, new Vector3(spawnRadiusX * 2, spawnRadiusY * 2, 0));
    }

    private void SpawnMegaCoin()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Vector3 centerPoint = player != null ? player.transform.position : transform.position;

        float randomX = Random.Range(-spawnRadiusX, spawnRadiusX);
        float randomY = Random.Range(-spawnRadiusY, spawnRadiusY);
        Vector3 spawnPosition = centerPoint + new Vector3(randomX, randomY, 0f);

        Instantiate(megaCoinPrefab, spawnPosition, Quaternion.identity);
        Debug.Log($"💰 ¡Apareció la Mega Moneda de la ronda {currentWave}!");
    }
}