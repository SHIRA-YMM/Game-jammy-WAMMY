using UnityEngine;
using System;

public class GlobalCoinManager : MonoBehaviour
{
    public static GlobalCoinManager Instance { get; private set; }

    public int coins;
    public static event Action OnCoinChanged; // event

    void Awake()
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

    public void AddCoins(int amount)
    {
        coins += amount;
        SaveCoins();
        OnCoinChanged?.Invoke();
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            SaveCoins();
            OnCoinChanged?.Invoke();
            return true;
        }
        return false;
    }

    public void SaveCoins() => PlayerPrefs.SetInt("GlobalCoins", coins);
    public void LoadCoins() => coins = PlayerPrefs.GetInt("GlobalCoins", 0);
}
