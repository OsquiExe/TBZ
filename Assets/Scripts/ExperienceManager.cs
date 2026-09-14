using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExperienceManager : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] private AnimationCurve experienceCurve;

    private int currentLevel = 1;
    private int maxExperience;
    private int previousLevelsExperience, nextLevelsExperience;


    private int pendingLevelUps = 0;

    [Header("Interface")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image experienceFill;

    public delegate void ExperienceChangeHandler(int amount);
    public event ExperienceChangeHandler OnExperienceChange;

    public static ExperienceManager Instance;

    private GameState currentState;

    public enum GameState
    {
        Playing,
        PowerUpSelection
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentState = GameState.Playing;
        UpdateLevel();
    }

    public void AddExperience(int amount)
    {
        OnExperienceChange?.Invoke(amount);
        maxExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
    }

    public void CheckForLevelUp()
    {
        bool leveledUp = false;

     
        while (maxExperience >= nextLevelsExperience && experienceCurve.length > 0)
        {
            currentLevel++;
            UpdateLevel();
            pendingLevelUps++; 
            leveledUp = true;
        }

   
        if (leveledUp && currentState == GameState.Playing)
        {
            ProcessNextLevelUp();
        }
    }

 
    public void ProcessNextLevelUp()
    {
        if (pendingLevelUps > 0)
        {
            pendingLevelUps--;
            ChangeState(GameState.PowerUpSelection);
        }
        else
        {
            
            ChangeState(GameState.Playing);
        }
    }

    public void UpdateLevel()
    {
        previousLevelsExperience = (int)experienceCurve.Evaluate(currentLevel - 1);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel);
        UpdateInterface();
    }

    public void UpdateInterface()
    {
        int start = maxExperience - previousLevelsExperience;
        int end = nextLevelsExperience - previousLevelsExperience;

        if (levelText != null) levelText.text = currentLevel.ToString();
        if (experienceFill != null)
        {
            float progress = (end > 0) ? (float)start / (float)end : 0f;
            experienceFill.fillAmount = Mathf.Clamp01(progress);
        }
    }

    public int GetCurrentLevel()
    {
        return currentLevel;
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                if (PowerUpManager.Instance != null)
                {
                    PowerUpManager.Instance.HidePowerUpSelection();
                }
                break;

            case GameState.PowerUpSelection:
                Time.timeScale = 0f;
                if (PowerUpManager.Instance != null)
                {
                    PowerUpManager.Instance.ShowPowerUpSelection();
                }
                break;
        }
    }
}