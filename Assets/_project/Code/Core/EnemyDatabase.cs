using System.Collections.Generic;
using UnityEngine;

public class EnemyDatabase : MonoBehaviour
{
    public static EnemyDatabase Instance;

    [SerializeField] private EnemyDataSO[] enemies;

    private Dictionary<EnemyType, EnemyDataSO> _enemyDictionary;

    public Dictionary<EnemyType, EnemyDataSO> EnemyDictionary => _enemyDictionary;

    private void Awake()
    {
        Instance = this;

        _enemyDictionary = new Dictionary<EnemyType, EnemyDataSO>();

        foreach (EnemyDataSO enemy in enemies)
        {
            if (!_enemyDictionary.ContainsKey(enemy.enemyType))
            {
                _enemyDictionary.Add(enemy.enemyType, enemy);
            }
        }
    }

    public EnemyDataSO GetEnemyData(EnemyType type)
    {
        return _enemyDictionary[type];
    }
}