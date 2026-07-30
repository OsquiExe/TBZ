using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Esencial para controlar los textos modernos de Unity

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Monedas")]
    [SerializeField] private TextMeshProUGUI dibiCoinsText; // Arrastrá acá tu texto de "Dibi-Coins"

    [Header("Paneles de UI (Opcional para el futuro)")]
    public GameObject settingsPanel;
    public GameObject storePanel;

    private void Start()
    {
        Time.timeScale = 1f;

        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (storePanel != null) storePanel.SetActive(false);

        UpdateCoinsUI();
    }

    private void OnEnable()
    {
        // Nos suscribimos al evento: si la plata cambia mientras estamos acá, se actualiza solo
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged += UpdateCoinsUI;
        }
    }

    private void OnDisable()
    {
        // Siempre es buena práctica desuscribirse al apagar el objeto
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.OnCoinsChanged -= UpdateCoinsUI;
        }
    }

    // La función que escribe el número en la pantalla
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
                // Por si arrancas el menú y el CurrencyManager no se cargó todavía
                dibiCoinsText.text = PlayerPrefs.GetInt("DibiCoinsTotal", 0).ToString();
            }
        }
    }

    // --- MÉTODOS PARA LOS BOTONES PRINCIPALES ---

    public void EmpezarMision()
    {
        Debug.Log("Cargando el nivel...");
        SceneManager.LoadScene("Level1");
    }

    public void CargarEscena(string nombreEscena)
    {
        SceneManager.LoadScene(nombreEscena);
    }

    // --- MÉTODOS PARA BOTONES SECUNDARIOS ---

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

    // --- MÉTODO PARA SALIR ---
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}