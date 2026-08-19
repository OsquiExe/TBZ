using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PowerUpManager : MonoBehaviour
{
    [Header("UI & References")]
    [SerializeField] public GameObject uiPowerUpSelect;
    [SerializeField] public GameObject powerUpPrefab;
    [SerializeField] private Transform powerUpPosition1;
    [SerializeField] private Transform powerUpPosition2;
    [SerializeField] private Transform powerUpPosition3;

    [Header("Reroll System")]
    [SerializeField] private TextMeshProUGUI rerollButtonText;
    private int totalRerollsUsed = 0;
    private const int maxFreeRerolls = 3;

    [Header("Active PowerUps UI (Lateral)")]
    [SerializeField] private Sprite rayoOff;
    [SerializeField] private Sprite rayoOn;

    [SerializeField] private powerUpDeck[] activePowerUpSlots;
    private int currentSlotIndex = 0;

    [System.Serializable]
    public struct powerUpDeck {
        public Image activePowerUp;
        public Image[] rayos;
    }

    [Header("Settings del Inventario")]
    [SerializeField] private List<PowerUpSO> deck;
    [SerializeField] private int maxPowerUpSlots = 4;
    [SerializeField] private int maxLevelPerPowerUp = 4;

    [Header("Referencias de Input a Apagar")]
    [SerializeField] private GameObject zonaTactilJoystick;

    private GameObject powerUp1, powerUp2, powerUp3;
    private Dictionary<PowerUpSO, int> ownedPowerUps = new Dictionary<PowerUpSO, int>();

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

    private void Update() {
#if UNITY_EDITOR
        if(Input.GetKeyDown(KeyCode.P)) {
            ExperienceManager.Instance.AddExperience(10);
        }
#endif
    }

    public void ShowPowerUpSelection()
    {
        if (TutorialManager.Instance != null && !TutorialManager.Instance.hasShownLevelUpTutorial)
        {
            TutorialManager.Instance.TriggerLevelUpTutorial(OpenPowerUpMenu);
        }
        else
        {
            OpenPowerUpMenu();
        }
    }

    private void OpenPowerUpMenu()
    {
        if (uiPowerUpSelect != null) uiPowerUpSelect.SetActive(true);

        Time.timeScale = 0f;

        if (zonaTactilJoystick != null)
        {
            VirtualJoystick vj = zonaTactilJoystick.GetComponent<VirtualJoystick>();
            if (vj != null) vj.ResetJoystick();

            zonaTactilJoystick.SetActive(false);
        }

        UpdateRerollButtonText();
        RandomizeNewPowerUps();
    }

    public void HidePowerUpSelection()
    {
        if (uiPowerUpSelect != null) uiPowerUpSelect.SetActive(false);

        if (zonaTactilJoystick != null) zonaTactilJoystick.SetActive(true);

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

        if (deck == null || deck.Count == 0) return;

        List<PowerUpSO> availablePowerUps = new List<PowerUpSO>(deck);
        int currentLevel = ExperienceManager.Instance != null ? ExperienceManager.Instance.GetCurrentLevel() : 1;

        availablePowerUps.RemoveAll(powerUp =>
        {
            bool isNewAndInventoryFull = !ownedPowerUps.ContainsKey(powerUp) && ownedPowerUps.Count >= maxPowerUpSlots;
            bool isAtMaxLevel = ownedPowerUps.ContainsKey(powerUp) && ownedPowerUps[powerUp] >= maxLevelPerPowerUp;
            bool levelNotReached = powerUp.unlockLevel > currentLevel;
            bool isUniqueAndOwned = powerUp.isUnique && ownedPowerUps.ContainsKey(powerUp);

            return isNewAndInventoryFull || isAtMaxLevel || levelNotReached || isUniqueAndOwned;
        });

        if (availablePowerUps.Count == 0)
        {
            SkipPowerUp();
            return;
        }

        List<PowerUpSO> randomizePowerUps = new List<PowerUpSO>();
        int optionsToCreate = Mathf.Min(3, availablePowerUps.Count);

        while (randomizePowerUps.Count < optionsToCreate)
        {
            PowerUpSO randomPowerUp = availablePowerUps[UnityEngine.Random.Range(0, availablePowerUps.Count)];

            if (!randomizePowerUps.Contains(randomPowerUp)) randomizePowerUps.Add(randomPowerUp);
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
        if (powerUpScript != null) powerUpScript.Setup(powerUpSO);
        return powerUpGo;
    }

    public void OnRerollButtonClicked()
    {
        if (totalRerollsUsed >= maxFreeRerolls)
        {
            Debug.Log("📺 [MONETIZACIÓN] Mostrando Anuncio de Recompensa (Rewarded Ad)...");
            ExecuteReroll();
        }
        else
        {
            ExecuteReroll();
        }
    }

    private void ExecuteReroll()
    {
        totalRerollsUsed++;
        UpdateRerollButtonText();
        RandomizeNewPowerUps();
    }

    private void UpdateRerollButtonText()
    {
        if (rerollButtonText == null) return;

        if (totalRerollsUsed >= maxFreeRerolls)
        {
            rerollButtonText.text = "Reroll (Ver Ad)";
        }
        else
        {
            int remainingFree = maxFreeRerolls - totalRerollsUsed;
            rerollButtonText.text = $"Reroll ({remainingFree} Gratis)";
        }
    }

    public void SelectPowerUp(PowerUpSO selectedPowerUp)
    {
        if (ownedPowerUps.ContainsKey(selectedPowerUp))
        {
            ownedPowerUps[selectedPowerUp]++;
            int newLevel = ownedPowerUps[selectedPowerUp];
            Debug.Log($"⬆️ PowerUp [{selectedPowerUp.powerUpText}] subió al nivel {newLevel}");

            UpdatePowerUpEffect(selectedPowerUp, newLevel);
        }
        else
        {
            ownedPowerUps.Add(selectedPowerUp, 1);
            Debug.Log($" Nuevo PowerUp adquirido: [{selectedPowerUp.powerUpText}] (Nivel 1)");
            UpdateActivePowerUpUI(selectedPowerUp);

            ApplyNewPowerUpEffect(selectedPowerUp);
        }

        HidePowerUpSelection();
        Time.timeScale = 1f;

        if (ExperienceManager.Instance != null) ExperienceManager.Instance.ProcessNextLevelUp();
    }

    // INYECTOR DE SCRIPTS
    private void ApplyNewPowerUpEffect(PowerUpSO powerUp)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        // 1. Cura Progresiva
        if (powerUp.name.Contains("Progressive"))
        {
            ProgressiveHealEffect healEffect = player.gameObject.AddComponent<ProgressiveHealEffect>();
            healEffect.level = 1;
        }

        // 2. Potenciador de Daño
        if (powerUp.name.Contains("Damage"))
        {
            DamageIncreaseEffect dmgEffect = player.gameObject.AddComponent<DamageIncreaseEffect>();
            dmgEffect.level = 1;
        }

        // 3. Onda Expansiva
        if (powerUp.name.Contains("Shock") || powerUp.name.Contains("Wave"))
        {
            ShockwaveEffect shockEffect = player.gameObject.AddComponent<ShockwaveEffect>();
            shockEffect.level = 1;
        }

        // 4. Misiles (Antes Capybara Bomb)
        if (powerUp.name.Contains("Decoys"))
        {
            MissileEffect missileEffect = player.gameObject.AddComponent<MissileEffect>();
            missileEffect.level = 1;
        }

        //Spawn the power Up
        Transform playerTransform = PlayerController.GetPlayerTransform?.Invoke();
        if(playerTransform == null) {
            Debug.LogWarning("No hay ningun jugador en la escena!");

            return;
        }

        if(powerUp.prefab != null) {
            Instantiate(powerUp.prefab, playerTransform);
        }

    }

    // ACTUALIZADOR DE NIVELES
    private void UpdatePowerUpEffect(PowerUpSO powerUp, int newLevel)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return;

        // 1. Cura Progresiva
        if (powerUp.name.Contains("Progressive"))
        {
            ProgressiveHealEffect healEffect = player.GetComponent<ProgressiveHealEffect>();
            if (healEffect != null) healEffect.level = newLevel;
        }

        // 2. Potenciador de Daño
        if (powerUp.name.Contains("Damage"))
        {
            DamageIncreaseEffect dmgEffect = player.GetComponent<DamageIncreaseEffect>();
            if (dmgEffect != null) dmgEffect.level = newLevel;
        }

        // 3. Onda Expansiva
        if (powerUp.name.Contains("Shock") || powerUp.name.Contains("Wave"))
        {
            ShockwaveEffect shockEffect = player.GetComponent<ShockwaveEffect>();
            if (shockEffect != null) shockEffect.level = newLevel;
        }

        // 4. Misiles
        if (powerUp.name.Contains("Decoys"))
        {
            MissileEffect missileEffect = player.GetComponent<MissileEffect>();
            if (missileEffect != null) missileEffect.level = newLevel;
        }

        if(powerUp.prefab == null) return;
        Type powerUpClass = powerUp.prefab.GetComponent<PowerUpClass>().GetType();
        var PowerUpObject = (PowerUpClass)player.GetComponentInChildren(powerUpClass, true);
        PowerUpObject.Upgrade(newLevel);

        //Update UI

        for(int i = 0; i < activePowerUpSlots.Length; i++) {
            if(activePowerUpSlots[i].activePowerUp.sprite != powerUp.powerUpIconRound) continue;

            int rayosCount = activePowerUpSlots[i].rayos.Length;
            for(int j = 0; j < rayosCount; j++) {
                activePowerUpSlots[i].rayos[j].sprite = newLevel <= j ? rayoOff : rayoOn;
            }
        }
    }

    public void SkipPowerUp()
    {
        HidePowerUpSelection();
        Time.timeScale = 1f;    

        if (ExperienceManager.Instance != null)
        {
            ExperienceManager.Instance.ProcessNextLevelUp();
        }
    }

    private void UpdateActivePowerUpUI(PowerUpSO powerUp)
    {
        if (activePowerUpSlots != null && currentSlotIndex < activePowerUpSlots.Length)
        {
            Image currentSlot = activePowerUpSlots[currentSlotIndex].activePowerUp;
            if (currentSlot != null && powerUp.powerUpIconRound != null)
            {
                currentSlot.sprite = powerUp.powerUpIconRound;
                Color slotColor = currentSlot.color;
                slotColor.a = 1f;
                currentSlot.color = slotColor;

                activePowerUpSlots[currentSlotIndex].rayos[0].sprite = rayoOn;
            }
            currentSlotIndex++;
        }
    }

    public int GetSelectedPowerUpsCount() { return ownedPowerUps.Count; }

    public int GetPowerUpLevel(string powerUpFileName)
    {
        foreach (var item in ownedPowerUps)
        {
            if (item.Key.name.Contains(powerUpFileName))
            {
                return item.Value;
            }
        }
        return 0;
    }
}