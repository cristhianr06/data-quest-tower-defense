using System;
using PrimeTween;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public event Action EnemyArrived;

    public event Action<int> OnWaypointChanged;

    private EnemyDataSO enemyData;

    private EnemyHealth enemyHealth;

    private Transform[] waypoints;

    private Transform cachedTransform;

    private Tween rotationTween;

    private bool isArrived;

    private int currentWaypointIndex = 1;

    // =========================================================
    // PROPERTY
    // =========================================================

    public int CurrentWaypointIndex
    {
        get => currentWaypointIndex;

        private set
        {
            if (currentWaypointIndex != value)
            {
                currentWaypointIndex = value;

                OnWaypointChanged?.Invoke(
                    currentWaypointIndex);
            }
        }
    }

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        enemyHealth =
            GetComponent<EnemyHealth>();

        cachedTransform = transform;

        Waypoints waypointsObject =
            FindFirstObjectByType<Waypoints>();

        if (waypointsObject != null)
        {
            waypoints =
                waypointsObject.wayPointsTransform;
        }
    }

    private void Update()
    {
        Movement();
    }

    // =========================================================
    // INITIALIZE
    // =========================================================

    public void Initialize(EnemyDataSO data)
    {
        enemyData = data;

        CurrentWaypointIndex = 1;

        isArrived = false;

        RotationLook();
    }

    // =========================================================
    // RESET
    // =========================================================

    public void ResetEnemy()
    {
        Tween.StopAll(cachedTransform);

        CurrentWaypointIndex = 1;

        isArrived = false;

        cachedTransform.position =
            waypoints[0].position;
    }

    // =========================================================
    // MOVEMENT
    // =========================================================

    private void Movement()
    {
        if (enemyData == null)
            return;

        if (enemyHealth.IsDied)
            return;

        if (waypoints == null ||
            waypoints.Length == 0 ||
            isArrived)
        {
            return;
        }

        if (currentWaypointIndex >= waypoints.Length)
            return;

        cachedTransform.position =
            Vector3.MoveTowards(
                cachedTransform.position,
                waypoints[currentWaypointIndex].position,
                enemyData.moveSpeed * Time.deltaTime);

        float distance =
            (waypoints[currentWaypointIndex].position -
             cachedTransform.position)
            .sqrMagnitude;

        if (distance <=
            enemyData.toleranceDistance *
            enemyData.toleranceDistance)
        {
            if (currentWaypointIndex >=
                waypoints.Length - 1)
            {
                isArrived = true;

                EnemyArrived?.Invoke();

                CoreHealth.Instance.TakeDamage(
                    enemyData.damageToCore);

                return;
            }

            CurrentWaypointIndex++;

            RotationLook();
        }
    }

    // =========================================================
    // ROTATION
    // =========================================================

    private void RotationLook()
    {
        if (enemyData == null)
            return;

        if (isArrived)
            return;

        if (waypoints == null ||
            waypoints.Length == 0)
        {
            return;
        }

        if (currentWaypointIndex >= waypoints.Length)
            return;

        if (rotationTween.isAlive)
        {
            rotationTween.Stop();
        }

        Vector3 direction =
            (waypoints[currentWaypointIndex].position -
             cachedTransform.position)
            .normalized;

        if (direction == Vector3.zero)
            return;

        Quaternion look =
            Quaternion.LookRotation(direction);

        rotationTween =
            Tween.Rotation(
                cachedTransform,
                look,
                enemyData.rotationSpeed,
                Ease.OutQuad);
    }
}