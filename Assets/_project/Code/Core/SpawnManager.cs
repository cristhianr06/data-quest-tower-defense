using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float delayStartFirstWave = 2f;

    [SerializeField] private float delayBetweenWaves = 5f;

    public TextMeshProUGUI counterWaveTMP;

    public static SpawnManager Instance;

    [Header("Wave Config")]
    public WaveSO[] listWave;

    [Header("Spawn")]
    public Transform spawnPoint;

    private Queue<GameObject> _enemyQueue;

    private WaitForSeconds _wait;

    private WaitForSeconds _waitBetweenWaves;

    private WaitForSeconds _waitFirstWave;

    private int _currentWaveIndex = 0;

    private int _enemyIdCounter = 0;

    private int _enemyAmountWave;

    // =========================================================
    // ACTIVE ENEMIES
    // =========================================================

    private int _currentAliveEnemies = 0;

    // =========================================================
    // EVENTS
    // =========================================================

    public event Action<List<string>, int, int> OnQueueCreated;

    public event Action OnEnemyDequeued;

    public event Action OnQueueCleared;

    private void Awake()
    {
        Instance = this;

        _waitBetweenWaves =
            new WaitForSeconds(delayBetweenWaves);

        _waitFirstWave =
            new WaitForSeconds(delayStartFirstWave);
    }

    private void Start()
    {
        StartCoroutine(PrepareWaveRoutine());
    }

    private IEnumerator PrepareWaveRoutine()
    {
        yield return _waitFirstWave;

        NextWave();
    }

    private IEnumerator NextWaveDelayRoutine()
    {
        yield return _waitBetweenWaves;

        NextWave();
    }

    // =========================================================
    // NEXT WAVE
    // =========================================================

    private void NextWave()
    {
        _enemyAmountWave = 0;

        if (listWave == null || listWave.Length == 0)
            return;

        if (_currentWaveIndex >= listWave.Length)
        {
            Debug.Log("ALL WAVES COMPLETED");
            return;
        }

        counterWaveTMP.text =
            $"Oleada: {_currentWaveIndex + 1}";

        // =========================================================
        // CREATE FIFO QUEUE
        // =========================================================

        _enemyQueue = new Queue<GameObject>(
            listWave[_currentWaveIndex].enemiesInWave);

        _wait = new WaitForSeconds(
            listWave[_currentWaveIndex].spawnInterval);

        _enemyAmountWave = _enemyQueue.Count;

        // =========================================================
        // BUILD UI SNAPSHOT
        // =========================================================

        List<string> queueSnapshot =
            BuildQueueSnapshot();

        OnQueueCreated?.Invoke(
            queueSnapshot,
            _currentWaveIndex,
            _enemyAmountWave);

        StartCoroutine(SpawnRoutine());

        _currentWaveIndex++;
    }

    // =========================================================
    // SPAWN ROUTINE
    // =========================================================

    private IEnumerator SpawnRoutine()
    {
        while (_enemyQueue.Count > 0)
        {
            GameObject nextEnemy =
                _enemyQueue.Dequeue();

            _enemyIdCounter++;

            string cleanName =
                nextEnemy.name.Replace("(Clone)", "");

            OnEnemyDequeued?.Invoke();

            GameObject spawnedEnemy =
                LeanPool.Spawn(
                    nextEnemy,
                    spawnPoint.position,
                    spawnPoint.rotation);

            EnemyController enemyController =
                spawnedEnemy.GetComponent<EnemyController>();

            enemyController.Initialize(
                _enemyIdCounter,
                cleanName);

            // =========================================================
            // TRACK ALIVE ENEMIES
            // =========================================================

            _currentAliveEnemies++;

            enemyController.OnEnemyRemoved +=
                HandleEnemyRemoved;

            yield return _wait;
        }

        OnQueueCleared?.Invoke();

        CheckWaveCompleted();
    }

    // =========================================================
    // ENEMY REMOVED
    // =========================================================

    private void HandleEnemyRemoved(
        EnemyController enemy)
    {
        _currentAliveEnemies--;

        enemy.OnEnemyRemoved -= HandleEnemyRemoved;

        CheckWaveCompleted();
    }

    // =========================================================
    // CHECK WAVE COMPLETED
    // =========================================================

    private void CheckWaveCompleted()
    {
        bool queueEmpty =
            _enemyQueue == null ||
            _enemyQueue.Count == 0;

        bool noEnemiesAlive =
            _currentAliveEnemies <= 0;

        if (queueEmpty && noEnemiesAlive)
        {
            //Debug.Log("WAVE COMPLETED");

            StartCoroutine(
                NextWaveDelayRoutine());
        }
    }

    // =========================================================
    // SNAPSHOT
    // =========================================================

    public List<string> GetCurrentQueueSnapshot()
    {
        return BuildQueueSnapshot();
    }

    private List<string> BuildQueueSnapshot()
    {
        List<string> snapshot = new();

        if (_enemyQueue == null)
            return snapshot;

        int previewId = _enemyIdCounter;

        foreach (GameObject enemy in _enemyQueue)
        {
            previewId++;

            string cleanName =
                enemy.name.Replace("(Clone)", "");

            string displayName =
                $"{cleanName}_{previewId}";

            snapshot.Add(displayName);
        }

        return snapshot;
    }

    private void OnDestroy()
    {
        if (_enemyQueue != null)
        {
            _enemyQueue.Clear();
        }
    }
}