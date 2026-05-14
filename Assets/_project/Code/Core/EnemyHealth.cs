using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamage
{
    public EnemyDataSO enemyData;
    public Image healthBar;
    public event Action OnDead;

    public float currentHealth;
    public bool _isDied;

    private EnemyAnimation _enemyAnimation;

    private void Awake()
    {
        _enemyAnimation = GetComponent<EnemyAnimation>();
    }

    public void TakeDamage(float damage)
    {
        if (_isDied) return;
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        healthBar.fillAmount = currentHealth / enemyData.maxHealth;

        if(currentHealth <= 0f)
        {
            _isDied = true;

            PlayerEconomy.Instance.AddGold(enemyData.goldReward);

            _enemyAnimation.DeadAnimation();

            OnDead?.Invoke();
        }
    }
}
