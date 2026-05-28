using System;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamage
{
    public Slider healthBar;

    public event Action<EnemyHealth> OnDead;

    public float currentHealth;

    public bool IsDied { get; private set; }

    private EnemyAnimation enemyAnimation;

    private EnemyDataSO enemyData;

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        enemyAnimation =
            GetComponent<EnemyAnimation>();
    }

    // =========================================================
    // INITIALIZE
    // =========================================================

    public void Initialize(EnemyDataSO data)
    {
        enemyData = data;

        IsDied = false;

        currentHealth =
            enemyData.maxHealth;

        healthBar.maxValue =
            enemyData.maxHealth;

        healthBar.value =
            currentHealth;
    }

    // =========================================================
    // DAMAGE
    // =========================================================

    public void TakeDamage(float damage)
    {
        if (enemyData == null)
            return;

        if (IsDied)
            return;

        currentHealth -= damage;

        healthBar.value = currentHealth;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // =========================================================
    // DIE
    // =========================================================

    private void Die()
    {
        IsDied = true;

        PlayerEconomy.Instance.AddGold(
            enemyData.goldReward);

        enemyAnimation.DeadAnimation();

        if (enemyData.deathFX != null)
        {
            LeanPool.Spawn(
                enemyData.deathFX,
                transform.position,
                Quaternion.identity);
        }

        OnDead?.Invoke(this);
    }
}