using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Para reiniciar el nivel

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Paneles de UI")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject defeatPanel;
    [SerializeField] private GameObject victoryPanel;

    private bool isGameOver = false;
    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        // Asegurarnos de que los paneles empiecen apagados y el tiempo corriendo
        if (pausePanel) pausePanel.SetActive(false);
        if (defeatPanel) defeatPanel.SetActive(false);
        if (victoryPanel) victoryPanel.SetActive(false);
        Time.timeScale = 1f;
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
        if (defeatPanel) defeatPanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void TriggerVictory()
    {
        if (isGameOver) return;

        isGameOver = true;
        if (victoryPanel) victoryPanel.SetActive(true);
        Time.timeScale = 0f; 
    }


    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}