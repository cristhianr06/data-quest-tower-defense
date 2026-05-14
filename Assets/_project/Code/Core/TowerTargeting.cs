using System.Collections.Generic;
using UnityEngine;

public class TowerTargeting : MonoBehaviour
{
    public TowerDataSO towerData;

    public Transform shootPoint;

    private float _fireCountdown;

    public List<EnemyHealth> enemiesInRange = new();

    private EnemyHealth _currentTarget;

    private void Update()
    {
        UpdateTarget();

        if (_currentTarget == null)
            return;

        RotateToTarget();

        HandleShooting();
    }

    private void UpdateTarget()
    {
        enemiesInRange.RemoveAll(e => e == null || e._isDied);

        if (enemiesInRange.Count == 0)
        {
            _currentTarget = null;
            return;
        }

        _currentTarget = enemiesInRange[0];
    }

    private void HandleShooting()
    {
        _fireCountdown -= Time.deltaTime;

        if (_fireCountdown <= 0f)
        {
            Shoot();

            _fireCountdown = 1f / towerData.fireRate;
        }
    }

    private void Shoot()
    {
        GameObject projectileObj = Lean.Pool.LeanPool.Spawn(
            towerData.projectilePrefab,
            shootPoint.position,
            Quaternion.identity);

        Projectile projectile = projectileObj.GetComponent<Projectile>();

        projectile.SetTarget(_currentTarget.transform, towerData.damage);
    }

    private void RotateToTarget()
    {
        Vector3 direction = _currentTarget.transform.position - transform.position;

        direction.y = 0;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Lerp(
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
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out EnemyHealth enemy))
        {
            enemiesInRange.Remove(enemy);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, towerData.range);
    }
}