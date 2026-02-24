using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class ExperienceManager : MonoBehaviour
{
    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve;

    int currentLevel, maxExperience;
    int previousLevelsExperience, nextLevelsExperience;

    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] Image experienceFill;

    //Singleton
    public static ExperienceManager Instance;

    public delegate void ExperienceChangeHandler( int amount);
    public event ExperienceChangeHandler OnExperienceChange;

    //[Header("Power Ups")]
    //[SerializeField] public PowerUpManager powerUpManager; 

    void Start()
    {
        UpdateLevel();
    }

    //Singleton Check
    private void Awake()
    {
        if( Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void AddExperience( int amount)
    {
        OnExperienceChange?.Invoke(amount);
        maxExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
    }

    public void CheckForLevelUp()
    {
        if( maxExperience >= nextLevelsExperience)
        {
            currentLevel ++;
            //powerUpManager.ShowPowerUps();
            UpdateLevel();
        }
    }

    public void UpdateLevel()
    {
        previousLevelsExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
        UpdateInterface();
    }

    public void UpdateInterface()
    {
        int start = maxExperience - previousLevelsExperience;
        int end = nextLevelsExperience - previousLevelsExperience;

        levelText.text = currentLevel.ToString();
        experienceFill.fillAmount = (float)start / (float) end; 
    }
}
