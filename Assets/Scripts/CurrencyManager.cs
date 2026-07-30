using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    // Esta es la "llave" con la que guardamos el progreso en el celular/PC
    private const string COIN_KEY = "DibiCoinsTotal";

    public int TotalCoins { get; private set; }

    // Este evento le avisa a la UI que la plata cambió, así se actualiza sola
    public Action OnCoinsChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Hace que este script sobreviva al cambiar de escena
            LoadCoins();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadCoins()
    {
        // Carga la plata guardada. Si es la primera vez que juega, arranca en 0.
        TotalCoins = PlayerPrefs.GetInt(COIN_KEY, 0);
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        PlayerPrefs.SetInt(COIN_KEY, TotalCoins);
        PlayerPrefs.Save();

        // Avisamos que hay nueva plata
        OnCoinsChanged?.Invoke();
    }

    // Ya te dejo preparada la función para cuando quieras comprar en la tienda
    public bool SpendCoins(int amount)
    {
        if (TotalCoins >= amount)
        {
            TotalCoins -= amount;
            PlayerPrefs.SetInt(COIN_KEY, TotalCoins);
            PlayerPrefs.Save();

            OnCoinsChanged?.Invoke();
            return true; // Compra exitosa
        }
        return false; // No le alcanza la plata
    }
}