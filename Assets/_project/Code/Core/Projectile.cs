using Lean.Pool;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    public float speed = 15f;

    private Transform _target;

    private float _damage;

    public GameObject impactFX;

    public void SetTarget(Transform target, float damage)
    {
        _target = target;
        _damage = damage;
    }

    private void Update()
    {
        if (_target == null)
        {
            LeanPool.Despawn(this);
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            _target.position,
            speed * Time.deltaTime);

        transform.LookAt(_target);

        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance <= 0.2f)
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (_target.TryGetComponent(out IDamage damageable))
        {
            damageable.TakeDamage(_damage);
        }

        if (impactFX != null)
        {
            Instantiate(impactFX, transform.position, Quaternion.identity);
        }

        LeanPool.Despawn(this);
    }

    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
        _target = null;
    }
}