using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Monedas")]
    [SerializeField] private TextMeshProUGUI dibiCoinsText;

    [Header("Paneles de UI (Opcional para el futuro)")]
    public GameObject settingsPanel;
    public GameObject storePanel;

    private void Start()
    {
        Time.timeScale = 1f;

        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (storePanel != null) storePanel.SetActive(false);

        UpdateCoinsUI();

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged += UpdateCoinsUI;
        }
    }

    private void OnDestroy()
    {

        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged -= UpdateCoinsUI;
        }
    }

    private void UpdateCoinsUI()
    {
        if (dibiCoinsText != null)
        {
            if (CurrencyManager.Instance != null)
            {
                dibiCoinsText.text = CurrencyManager.Instance.TotalCoins.ToString();
            }
            else
            {
                dibiCoinsText.text = PlayerPrefs.GetInt("DibiCoinsTotal", 0).ToString();
            }
        }
    }

    public void EmpezarMision()
    {
        Debug.Log("Cargando el nivel...");
        SceneManager.LoadScene("Level1");
    }

    public void CargarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    public void AbrirConfiguracion()
    {
        Debug.Log("Abriendo Configuración (Settings)");
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CerrarConfiguracion()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void AbrirTienda()
    {
        Debug.Log("Abriendo Store");
        if (storePanel != null) storePanel.SetActive(true);
    }

    public void AbrirRecompensasDiarias()
    {
        Debug.Log("Abriendo Daily Rewards");
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}