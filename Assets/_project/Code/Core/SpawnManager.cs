using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using EditorAttributes;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [Header("Wave Config")]
    public WaveSO[] listWave;

    [Header("Spawn")]
    public Transform spawnPoint;

    private Queue<GameObject> _enemyQueue;

    private WaitForSeconds _wait;

    private int _counterWave = -1;

    // PROPIEDAD PARA UI
    public int CurrentQueueCount => _enemyQueue != null ? _enemyQueue.Count : 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EnemyCounterWave.Instance.endCurrentWave += NextWave;

        PrepareWave();
    }

    [Button("Iniciar")]
    public void PrepareWave()
    {
        NextWave();
    }

    private void NextWave()
    {
        if (listWave == null || listWave.Length == 0)
            return;

        _counterWave++;

        if (_counterWave > listWave.Length - 1)
        {
            Debug.Log("Todas las oleadas completadas");
            return;
        }

        Debug.Log($"Counter Wave: {_counterWave}");

        // QUEUE
        _enemyQueue = new Queue<GameObject>(
            listWave[_counterWave].enemiesInWave);

        _wait = new WaitForSeconds(
            listWave[_counterWave].spawnInterval);

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        EnemyCounterWave.Instance.amountCurrentWave = _enemyQueue.Count;

        while (_enemyQueue.Count > 0)
        {
            GameObject nextEnemy = _enemyQueue.Dequeue();

            Debug.Log($"En cola: {_enemyQueue.Count}");

            LeanPool.Spawn(
                nextEnemy,
                spawnPoint.position,
                spawnPoint.rotation);

            yield return _wait;
        }
    }

    private void OnDestroy()
    {
        if (_enemyQueue != null && _enemyQueue.Count > 0)
        {
            _enemyQueue.Clear();
        }

        // AQUÍ TENÍAS EL BUG
        EnemyCounterWave.Instance.endCurrentWave -= NextWave;
    }
}