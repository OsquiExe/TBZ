using System; 
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

    GameState currentState;

    public delegate void ExperienceChangeHandler( int amount);
    public event ExperienceChangeHandler OnExperienceChange;
    public event Action<GameState> OnStateChanged;

    //[Header("Power Ups")]
    //[SerializeField] public PowerUpManager powerUpManager; 

    void Start()
    {
        UpdateLevel();
    }

    //Changes the game state from battle to power up selection
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ChangeState(GameState.PowerUpSelection);
            currentLevel++;
        }
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

    //calls the ExperienceChangeHandler event to increase the amount of experience necessary for level up
    public void AddExperience( int amount)
    {
        OnExperienceChange?.Invoke(amount);
        maxExperience += amount;
        CheckForLevelUp();
        UpdateInterface();
    }

    //Increase the current level to level up
    public void CheckForLevelUp()
    {
        if( maxExperience >= nextLevelsExperience)
        {
            currentLevel ++;
            //powerUpManager.ShowPowerUps();
            UpdateLevel();
        }
    }

    //evaluate both integer variables to update the next level
    public void UpdateLevel()
    {
        previousLevelsExperience = (int)experienceCurve.Evaluate(currentLevel);
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1);
        UpdateInterface();
    }

    //Update the values for previous and next level experience in the interface of the game
    public void UpdateInterface()
    {
        int start = maxExperience - previousLevelsExperience;
        int end = nextLevelsExperience - previousLevelsExperience;

        levelText.text = currentLevel.ToString();
        experienceFill.fillAmount = (float)start / (float) end; 
    }

    public int GetCurrentLevel()
    {
        return currentLevel; 
    }

    
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        OnStateChanged?.Invoke(newState);
        HandleStateChange();
    }

    
    public void HandleStateChange()
    {
        switch(currentState)
        {
            case GameState.Playing:
                 PowerUpManager.Instance.HidePowerUpSelection();
                 break;
            
            case GameState.PowerUpSelection:
                 PowerUpManager.Instance.ShowPowerUpSelection();
                 break;
        }
    }
    
    //storage for playing and power up selection game states
    public enum GameState
    {
        Playing,
        PowerUpSelection
    } 
}
