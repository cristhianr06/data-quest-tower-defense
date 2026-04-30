using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Lean.Pool;
using EditorAttributes;

public class SpawnManager : MonoBehaviour
{
    public WaveSO[] listWave;
    public Transform spawnPoint;

    private Queue<GameObject> _enemyQueue;
    private WaitForSeconds _wait;
    private int _counterWave = -1;


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
        if (listWave == null) return;
        _counterWave++;
        if (_counterWave > listWave.Length -1) return;
        Debug.Log($"Counter Wave: {_counterWave}");
        _enemyQueue = new Queue<GameObject>(listWave[_counterWave].enemiesInWave);
        _wait = new WaitForSeconds(listWave[_counterWave].spawnInterval);
        StartCoroutine(SpawnRoutine());
    }
    private IEnumerator SpawnRoutine()
    {
        EnemyCounterWave.Instance.amountCurrentWave = _enemyQueue.Count;
        while (_enemyQueue.Count > 0)
        {
            GameObject nextEnemy = _enemyQueue.Dequeue();
            Debug.Log($"cola: {_enemyQueue.Count}");
            GameObject enemy = LeanPool.Spawn(nextEnemy, spawnPoint.position, spawnPoint.rotation);
            yield return _wait;
        }
    }
    private void OnDestroy()
    {
        if(_enemyQueue.Count > 0)
            _enemyQueue.Clear();
        EnemyCounterWave.Instance.endCurrentWave += NextWave;
    }
}
