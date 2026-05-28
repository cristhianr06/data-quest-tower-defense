using System;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class TowerTargeting : MonoBehaviour
{
    [SerializeField] private GameObject towerArea;

    public GameObject TowerArea => towerArea;

    public TowerDataSO towerData;

    public Transform shootPoint;
    private AudioSource _audioSource;

    // =========================================================
    // OWNER
    // =========================================================

    public Tower OwnerTower { get; private set; }

    // =========================================================
    // EVENTS
    // =========================================================

    public event Action<EnemyHealth> OnEnemyAdded;

    public event Action<EnemyHealth> OnEnemyRemoved;

    // =========================================================
    // TARGETING
    // =========================================================

    private float fireCountdown;

    public List<EnemyHealth> enemiesInRange = new();

    private EnemyHealth currentTarget;

    // =========================================================
    // SET OWNER
    // =========================================================
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void SetOwnerTower(Tower tower)
    {
        OwnerTower = tower;
    }

    private void Update()
    {
        UpdateTarget();

        if (currentTarget == null)
            return;

        RotateToTarget();

        HandleShooting();
    }

    private void UpdateTarget()
    {
        if (enemiesInRange.Count == 0)
        {
            currentTarget = null;
            return;
        }

        currentTarget = enemiesInRange[0];
    }

    private void HandleShooting()
    {
        fireCountdown -= Time.deltaTime;

        if (fireCountdown <= 0f)
        {
            Shoot();

            fireCountdown =
                1f / towerData.fireRate;
        }
    }

    private void Shoot()
    {
        _audioSource.PlayOneShot(towerData.shootSFX);
        GameObject projectileObj =
            LeanPool.Spawn(
                towerData.projectilePrefab,
                shootPoint.position,
                Quaternion.identity);

        Projectile projectile =
            projectileObj.GetComponent<Projectile>();

        projectile.SetTarget(
            currentTarget.transform,
            towerData.damage);
    }

    private void RotateToTarget()
    {
        Vector3 direction =
            currentTarget.transform.position -
            transform.position;

        direction.y = 0;

        Quaternion lookRotation =
            Quaternion.LookRotation(direction);

        transform.rotation =
            Quaternion.Lerp(
                transform.rotation,
                lookRotation,
                Time.deltaTime * 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            if (!enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);

                enemy.OnDead += HandleEnemyDeath;

                OnEnemyAdded?.Invoke(enemy);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            enemy.OnDead -= HandleEnemyDeath;

            enemiesInRange.Remove(enemy);

            OnEnemyRemoved?.Invoke(enemy);
        }
    }

    private void HandleEnemyDeath(
        EnemyHealth enemy)
    {
        enemy.OnDead -= HandleEnemyDeath;

        enemiesInRange.Remove(enemy);

        OnEnemyRemoved?.Invoke(enemy);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            towerData.range);
    }
}