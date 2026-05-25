using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamage
{
    public EnemyDataSO enemyData;
    public Slider healthBar;
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
        healthBar.value = currentHealth;

        if(currentHealth <= 0.0f)
        {
            _isDied = true;

            PlayerEconomy.Instance.AddGold(enemyData.goldReward);

            _enemyAnimation.DeadAnimation();

            OnDead?.Invoke();
        }
    }
}
