using System;
using UnityEngine;

public class PlayerEconomy : MonoBehaviour
{
    public static PlayerEconomy Instance;

    public int startGold = 100;

    public int CurrentGold { get; private set; }

    public event Action<int> OnGoldChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CurrentGold = startGold;
        OnGoldChanged?.Invoke(CurrentGold);
    }

    public bool SpendGold(int amount)
    {
        if (CurrentGold < amount)
            return false;

        CurrentGold -= amount;

        OnGoldChanged?.Invoke(CurrentGold);

        return true;
    }

    public void AddGold(int amount)
    {
        CurrentGold += amount;

        OnGoldChanged?.Invoke(CurrentGold);
    }
}