using System;
using UnityEngine;
using UnityEngine.UI;

public class CoreHealth : MonoBehaviour
{
    public static CoreHealth Instance;

    public float maxHealth = 100f;

    public float currentHealth;

    public Slider healthBar;

    public event Action OnGameOver;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        healthBar.maxValue = maxHealth;
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateUI();

        if (currentHealth <= 0.0f)
        {
            OnGameOver?.Invoke();
        }
    }

    private void UpdateUI()
    {
        healthBar.value = currentHealth;
    }
}