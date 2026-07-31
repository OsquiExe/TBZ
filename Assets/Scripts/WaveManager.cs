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
    [SerializeField] private int bossWaveIndex = 18; 

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

        totalEnemiesThisWave = baseEnemies + ((currentWave - 1) * enemiesIncreasePerWave) + (powerUpCount * 3);

        spawnCooldownMultiplier = Mathf.Max(0.2f, 1f - (currentWave * 0.05f) - (powerUpCount * 0.05f));

        isWaveActive = true;
        UpdateUI();

        Debug.Log($"🌊 INICIA OLEADA {currentWave} | Enemigos totales: {totalEnemiesThisWave} | Vel. Spawn: {spawnCooldownMultiplier}x");

        if (currentWave == bossWaveIndex)
        {
            Debug.Log(" ¡ALERTA! OLEADA DEL JEFE FINAL ");
        }
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
            Debug.Log(" ¡Jefe derrotado! Victoria.");
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

    public float GetSpawnMultiplier()
    {
        return spawnCooldownMultiplier;
    }



    public float GetHealthMultiplier()
    {

        return 1f + (currentWave * 0.15f);
    }

    public float GetDamageMultiplier()
    {

        return 1f + (currentWave * 0.10f);
    }

    public float GetSpeedMultiplier()
    {

        return 1f + (currentWave * 0.02f);
    }
}