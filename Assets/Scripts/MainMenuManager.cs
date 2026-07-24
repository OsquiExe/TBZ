using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneles de UI (Opcional para el futuro)")]
    public GameObject settingsPanel;
    public GameObject storePanel;

    private void Start()
    {
    
        Time.timeScale = 1f;


        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (storePanel != null) storePanel.SetActive(false);
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