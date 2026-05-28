using System;
using System.Collections;
using Lean.Pool;
using UnityEngine;

public class EnemyController : MonoBehaviour, IPoolable
{
    public event Action<EnemyController> OnEnemyRemoved;

    [Header("Enemy Config")]
    public EnemyType enemyType;

    public GameObject enemyArea;

    public float delayDisable = 2f;

    public EnemyAnimation enemyAnimation;

    // =========================================================
    // RUNTIME
    // =========================================================

    public EnemyDataSO EnemyData { get; private set; }

    public int EnemyId { get; private set; }

    public string DisplayName { get; private set; }

    // =========================================================
    // COMPONENTS
    // =========================================================

    private EnemyMovement enemyMovement;

    private EnemyHealth enemyHealth;

    private WaitForSeconds wait;


    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        wait =
            new WaitForSeconds(delayDisable);

        enemyMovement =
            GetComponent<EnemyMovement>();

        enemyHealth =
            GetComponent<EnemyHealth>();
    }

    // =========================================================
    // INITIALIZE
    // =========================================================

    public void Initialize(
        int id,
        string enemyBaseName)
    {
        EnemyId = id;

        DisplayName =
            $"{enemyBaseName}_{id}";

        gameObject.name = DisplayName;

        // =====================================================
        // DICTIONARY LOOKUP
        // =====================================================

        EnemyData =
            EnemyDatabase.Instance
                .GetEnemyData(enemyType);

        if (EnemyData == null)
        {
            Debug.LogError(
                $"EnemyData missing for {enemyType}");

            return;
        }

        // =====================================================
        // INITIALIZE SYSTEMS
        // =====================================================

        enemyMovement.Initialize(EnemyData);

        enemyHealth.Initialize(EnemyData);

        // =====================================================
        // REGISTER EVENTS
        // =====================================================

        enemyHealth.OnDead -= DisableEnemy;
        enemyHealth.OnDead += DisableEnemy;

        enemyMovement.EnemyArrived -= EnemyArrived;
        enemyMovement.EnemyArrived += EnemyArrived;

        // =====================================================
        // VISUALS
        // =====================================================

        enemyAnimation.PlayWalkingAnim();

        Debug.Log(
            $"LOOKUP SUCCESS -> {enemyType}");
    }

    // =========================================================
    // POOL
    // =========================================================

    public void OnSpawn()
    {
        // LeanPool llama esto ANTES de Initialize()
        // No registrar lógica aquí.
    }

    public void OnDespawn()
    {
        enemyHealth.OnDead -= DisableEnemy;

        enemyMovement.EnemyArrived -= EnemyArrived;

        enemyMovement.ResetEnemy();

        StopAllCoroutines();
    }

    // =========================================================
    // ARRIVED
    // =========================================================

    private void EnemyArrived()
    {
        OnEnemyRemoved?.Invoke(this);

        LeanPool.Despawn(gameObject);
    }

    // =========================================================
    // DEAD
    // =========================================================

    private void DisableEnemy(
        EnemyHealth enemyHealth)
    {
        StartCoroutine(
            DisableEnemyCoroutine());
    }

    private IEnumerator DisableEnemyCoroutine()
    {
        yield return wait;

        OnEnemyRemoved?.Invoke(this);

        LeanPool.Despawn(gameObject);
    }
}