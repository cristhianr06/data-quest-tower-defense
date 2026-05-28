using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDatabase : MonoBehaviour
{
    public static EnemyDatabase Instance;

    [Header("Enemy Configurations")]
    [SerializeField]
    private EnemyDataSO[] enemyConfigs;

    // =========================================================
    // DICTIONARY
    // =========================================================

    private readonly Dictionary<EnemyType, EnemyDataSO>
        enemyDictionary = new();
    
    public event Action<EnemyType, EnemyDataSO>
        OnDictionaryLookup;

    public IReadOnlyDictionary<EnemyType, EnemyDataSO>
        EnemyDictionary => enemyDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        BuildDictionary();
    }

    // =========================================================
    // BUILD DICTIONARY
    // =========================================================

    private void BuildDictionary()
    {
        enemyDictionary.Clear();

        foreach (EnemyDataSO enemyData in enemyConfigs)
        {
            if (enemyData == null)
                continue;

            if (enemyDictionary.ContainsKey(
                    enemyData.enemyType))
            {
                Debug.LogWarning(
                    $"Duplicate EnemyType detected: {enemyData.enemyType}");

                continue;
            }

            enemyDictionary.Add(
                enemyData.enemyType,
                enemyData);

            Debug.Log(
                $"ADDED TO DICTIONARY -> {enemyData.enemyType}");
        }
    }

    // =========================================================
    // LOOKUP
    // =========================================================

    // public EnemyDataSO GetEnemyData(
    //     EnemyType enemyType)
    // {
    //     if (enemyDictionary.TryGetValue(
    //             enemyType,
    //             out EnemyDataSO enemyData))
    //     {
    //         return enemyData;
    //     }
    //
    //     Debug.LogError(
    //         $"EnemyData not found for type: {enemyType}");
    //
    //     return null;
    // }
    public EnemyDataSO GetEnemyData(
        EnemyType enemyType)
    {
        if (enemyDictionary.TryGetValue(
                enemyType,
                out EnemyDataSO enemyData))
        {
            // =====================================================
            // REAL-TIME LOOKUP EVENT
            // =====================================================

            OnDictionaryLookup?.Invoke(
                enemyType,
                enemyData);

            return enemyData;
        }

        Debug.LogError(
            $"EnemyData not found for type: {enemyType}");

        return null;
    }
}