using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEconomy : MonoBehaviour
{
    public static PlayerEconomy Instance;
    public Button crossbowButton;
    public Button cannonButton;
    public TowerDataSO crossbowData;
    public TowerDataSO cannonData;

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
        
        crossbowButton.interactable = CurrentGold >= crossbowData.cost;
        cannonButton.interactable = CurrentGold >= cannonData.cost;
        
        OnGoldChanged?.Invoke(CurrentGold);
    }

    public bool SpendGold(int amount)
    {
        
        CurrentGold -= amount;
        
        crossbowButton.interactable = CurrentGold >= crossbowData.cost;
        cannonButton.interactable = CurrentGold >= cannonData.cost;

        OnGoldChanged?.Invoke(CurrentGold);

        return true;
    }

    public void AddGold(int amount)
    {
        CurrentGold += amount;
        crossbowButton.interactable = CurrentGold >= crossbowData.cost;
        cannonButton.interactable = CurrentGold >= cannonData.cost;
        OnGoldChanged?.Invoke(CurrentGold);
    }
}