using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpManager : MonoBehaviour
{
    [Header("UI & References")]
    [SerializeField] public GameObject uiPowerUpSelect;
    [SerializeField] public GameObject powerUpPrefab;
    [SerializeField] private Transform powerUpPosition1;
    [SerializeField] private Transform powerUpPosition2;
    [SerializeField] private Transform powerUpPosition3;

    [Header("Active PowerUps UI (Lateral)")]
    [SerializeField] private Image[] activePowerUpSlots;
    private int currentSlotIndex = 0;

    [Header("Settings")]
    [SerializeField] private List<PowerUpSO> deck;

    private GameObject powerUp1, powerUp2, powerUp3;
    private List<PowerUpSO> alreadySelectedPowerUps = new List<PowerUpSO>();

    public static PowerUpManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        HidePowerUpSelection();
    }

    public void ShowPowerUpSelection()
    {
        if (uiPowerUpSelect != null)
        {
            uiPowerUpSelect.SetActive(true);
        }

        RandomizeNewPowerUps();
    }

    public void HidePowerUpSelection()
    {
        if (uiPowerUpSelect != null)
        {
            uiPowerUpSelect.SetActive(false);
        }

        ClearOldCards();
    }

    private void ClearOldCards()
    {
        if (powerUp1 != null) Destroy(powerUp1);
        if (powerUp2 != null) Destroy(powerUp2);
        if (powerUp3 != null) Destroy(powerUp3);
    }

    void RandomizeNewPowerUps()
    {
        ClearOldCards();

        if (deck == null || deck.Count == 0)
        {
            Debug.LogError("Error crìtico: El mazo de PowerUps en el script está vacío.");
            return;
        }

        List<PowerUpSO> availablePowerUps = new List<PowerUpSO>(deck);
        int currentLevel = ExperienceManager.Instance != null ? ExperienceManager.Instance.GetCurrentLevel() : 1;

        availablePowerUps.RemoveAll(powerUp =>
            (powerUp.isUnique && alreadySelectedPowerUps.Contains(powerUp)) ||
            (powerUp.unlockLevel > currentLevel)
        );

        if (availablePowerUps.Count == 0)
        {
            Debug.LogWarning("No quedan PowerUps válidos para dar. Procesando siguiente nivel o reanudando juego.");
            if (ExperienceManager.Instance != null)
            {
                ExperienceManager.Instance.ProcessNextLevelUp();
            }
            return;
        }

        List<PowerUpSO> randomizePowerUps = new List<PowerUpSO>();
        int optionsToCreate = Mathf.Min(3, availablePowerUps.Count);

        while (randomizePowerUps.Count < optionsToCreate)
        {
            PowerUpSO randomPowerUp = availablePowerUps[Random.Range(0, availablePowerUps.Count)];
            if (!randomizePowerUps.Contains(randomPowerUp))
            {
                randomizePowerUps.Add(randomPowerUp);
            }
        }

        if (optionsToCreate >= 1) powerUp1 = InstantiatePowerUp(randomizePowerUps[0], powerUpPosition1);
        if (optionsToCreate >= 2) powerUp2 = InstantiatePowerUp(randomizePowerUps[1], powerUpPosition2);
        if (optionsToCreate >= 3) powerUp3 = InstantiatePowerUp(randomizePowerUps[2], powerUpPosition3);
    }

    GameObject InstantiatePowerUp(PowerUpSO powerUpSO, Transform position)
    {
        if (position == null || powerUpPrefab == null) return null;

        GameObject powerUpGo = Instantiate(powerUpPrefab, position.position, Quaternion.identity, position);

        powerUpGo.transform.localPosition = Vector3.zero;

        PowerUp powerUpScript = powerUpGo.GetComponent<PowerUp>();
        if (powerUpScript != null)
        {
            powerUpScript.Setup(powerUpSO);
        }
        return powerUpGo;
    }

    public void SelectPowerUp(PowerUpSO selectedPowerUp)
    {
        if (!alreadySelectedPowerUps.Contains(selectedPowerUp))
        {
            alreadySelectedPowerUps.Add(selectedPowerUp);
        }

        UpdateActivePowerUpUI(selectedPowerUp);

        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.ProcessNextLevelUp();
        }
    }

    private void UpdateActivePowerUpUI(PowerUpSO powerUp)
    {
        if (activePowerUpSlots != null && currentSlotIndex < activePowerUpSlots.Length)
        {
            Image currentSlot = activePowerUpSlots[currentSlotIndex];

            if (currentSlot != null)
            {
                currentSlot.sprite = powerUp.powerUpImage;
                currentSlot.color = Color.white;
            }

            currentSlotIndex++;
        }
    }

    public int GetSelectedPowerUpsCount()
    {
        return alreadySelectedPowerUps.Count;
    }
}