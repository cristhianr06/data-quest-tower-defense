using Lean.Pool;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour, IPoolable
{
    public EnemyDataSO enemyData;
    public float delayDisable = 2f;
    private EnemyMovement _enemyMovement;
    public EnemyAnimation enemyAnimation;
    
    private EnemyHealth _enemyHealth;
    private WaitForSeconds _wait;
    private void Awake()
    {
        _wait = new WaitForSeconds(delayDisable);
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemyMovement = GetComponent<EnemyMovement>();
    }
    private void EnemyDespawn(int count)
    {
        LeanPool.Despawn(this);
    }

    public void OnSpawn()
    {
        enemyAnimation.PlayWalkingAnim();
        _enemyHealth.healthBar.fillAmount = 1f;
        _enemyHealth.currentHealth = enemyData.maxHealth;
        _enemyMovement.EnemyArrived += EnemyDespawn;
        _enemyMovement.EnemyArrived += EnemyCounterWave.Instance.OnReduceEnemies;
        _enemyHealth.OnDead += DisableEnemy;
    }
    public void OnDespawn()
    {
        _enemyHealth._isDied = false;
        _enemyMovement.ResetEnemy();
        _enemyMovement.EnemyArrived -= EnemyDespawn;
        _enemyMovement.EnemyArrived -= EnemyCounterWave.Instance.OnReduceEnemies;
        _enemyHealth.OnDead -= DisableEnemy;
    }
    public void DisableEnemy()
    {
        StartCoroutine(DisableEnemyCoroutine());
    }
    private IEnumerator DisableEnemyCoroutine()
    {
        yield return _wait;
        LeanPool.Despawn(this);
    }
}
