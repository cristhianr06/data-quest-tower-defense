using PrimeTween;
using System;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private EnemyDataSO _enemyData;
    public event Action<int> EnemyArrived;

    private int _currentWaypointIndex = 1;
    private Transform[] _waypoints;
    private Transform _transform;
    private bool _isArrived;
    private Tween _rotationTween;

    private EnemyHealth _enemyHealth;

    private void Awake()
    {
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemyData = GetComponent<EnemyController>().enemyData;
        _transform = transform;
        _waypoints = FindFirstObjectByType<Waypoints>().wayPointsTransform;
    }
    private void Start()
    {
        _currentWaypointIndex = 1;
        _isArrived = false;
        RotationLook();
    }
    public void ResetEnemy()
    {
        Tween.StopAll(this.transform);
        _currentWaypointIndex = 1;
        _isArrived = false;
    }
    private void Update()
    {
        Movement();
    }
    private void Movement()
    {
        if (_enemyHealth._isDied) return;
        if (_waypoints == null || _waypoints.Length == 0 || _isArrived) return;

        _transform.position = Vector3.MoveTowards(_transform.position, _waypoints[_currentWaypointIndex].position, _enemyData.moveSpeed * Time.deltaTime);

        float distance = (_waypoints[_currentWaypointIndex].position - _transform.position).sqrMagnitude;

        if (distance <= _enemyData.toleranceDistance * _enemyData.toleranceDistance)
        {
            if (_currentWaypointIndex >= _waypoints.Length - 1)
            {
                Debug.Log($"Llego: {gameObject.name}");
                _isArrived = true;
                EnemyArrived?.Invoke(1);
                return;
            }
            _currentWaypointIndex++;
            RotationLook();
        }
    }
    private void RotationLook()
    {
        if (_isArrived) return;

        if (_rotationTween.isAlive)
        {
            _rotationTween.Stop();
        }
        Vector3 direction = (_waypoints[_currentWaypointIndex].position - _transform.position).normalized;

        Quaternion look = Quaternion.LookRotation(direction);

        _rotationTween = Tween.Rotation(
            _transform,
            look,
            _enemyData.rotationSpeed,
            Ease.OutQuad);
    }
}
