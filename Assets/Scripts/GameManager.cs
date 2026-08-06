using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Paneles de UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject victoryPanel;

    [Header("Monedas del Nivel")]
    [SerializeField] private TextMeshProUGUI runCoinsText;
    public int coinsCollectedThisRun { get; private set; } = 0; 
    private bool coinsSaved = false; 

    private bool isGameOver = false;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);
        if (victoryPanel) victoryPanel.SetActive(false);
        Time.timeScale = 1f;

        UpdateRunCoinsUI();
    }


    public void AddCoinsToRun(int amount)
    {
        coinsCollectedThisRun += amount;
        UpdateRunCoinsUI();
    }

    private void UpdateRunCoinsUI()
    {
        if (runCoinsText != null)
        {
            runCoinsText.text = coinsCollectedThisRun.ToString();
        }
    }


    private void SaveRunCoinsToGlobal()
    {
        if (!coinsSaved && coinsCollectedThisRun > 0 && CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.AddCoins(coinsCollectedThisRun);
            coinsSaved = true;
            Debug.Log($"Depositando {coinsCollectedThisRun} monedas al banco global.");
        }
    }


    public void TogglePause()
    {
        if (isGameOver) return;

        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void TriggerDefeat()
    {
        if (isGameOver) return;

        isGameOver = true;
        SaveRunCoinsToGlobal();

        if (defeatPanel) defeatPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void TriggerVictory()
    {
        if (isGameOver) return;

        isGameOver = true;
        SaveRunCoinsToGlobal(); 

        if (victoryPanel) victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SaveRunCoinsToGlobal();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SaveRunCoinsToGlobal(); 
        SceneManager.LoadScene("MainMenu");
    }
}