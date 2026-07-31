using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;


    private const string COIN_KEY = "DibiCoinsTotal";

    public int TotalCoins { get; private set; }


    public Action OnCoinsChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            LoadCoins();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadCoins()
    {
   
        TotalCoins = PlayerPrefs.GetInt(COIN_KEY, 0);
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        PlayerPrefs.SetInt(COIN_KEY, TotalCoins);
        PlayerPrefs.Save();


        OnCoinsChanged?.Invoke();
    }


    public bool SpendCoins(int amount)
    {
        if (TotalCoins >= amount)
        {
            TotalCoins -= amount;
            PlayerPrefs.SetInt(COIN_KEY, TotalCoins);
            PlayerPrefs.Save();

            OnCoinsChanged?.Invoke();
            return true; 
        }
        return false; 
    }
}