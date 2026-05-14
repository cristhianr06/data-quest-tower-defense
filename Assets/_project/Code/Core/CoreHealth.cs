using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CoreHealth : MonoBehaviour
{
    public static CoreHealth Instance;

    public int maxHealth = 20;

    public int currentHealth;

    public Image healthBar;

    public event Action OnGameOver;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        currentHealth = Mathf.Max(currentHealth, 0);

        UpdateUI();

        if (currentHealth <= 0)
        {
            OnGameOver?.Invoke();
        }
    }

    private void UpdateUI()
    {
        healthBar.fillAmount = (float)currentHealth / maxHealth;
    }
}