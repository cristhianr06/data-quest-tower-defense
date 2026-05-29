using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using TMPro;

public class SpawnManager : MonoBehaviour
{
    [Header("Wave Delay")]
    [SerializeField] private float delayBetweenWaves = 5f;

    [Header("Countdown")]
    [SerializeField] private int countdownSeconds = 3;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI waveTMP;

    [SerializeField] private TextMeshProUGUI countdownTMP;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip spawnClip;

    public static SpawnManager Instance;

    [Header("Wave Config")]
    public WaveSO[] listWave;

    [Header("Spawn")]
    public Transform spawnPoint;

    private Queue<GameObject> enemyQueue;

    private WaitForSeconds waitEnemySpawn;

    private int currentWaveIndex = 0;

    private int enemyIdCounter = 0;

    private int enemyAmountWave;

    // =========================================================
    // ACTIVE ENEMIES
    // =========================================================

    private int currentAliveEnemies = 0;

    // =========================================================
    // EVENTS
    // =========================================================

    public event Action<List<string>, int, int> OnQueueCreated;

    public event Action OnEnemyDequeued;

    public event Action OnQueueCleared;

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        Instance = this;

        if (countdownTMP != null)
        {
            countdownTMP.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        StartCoroutine(StartFirstWaveRoutine());
    }

    // =========================================================
    // FIRST WAVE
    // =========================================================

    private IEnumerator StartFirstWaveRoutine()
    {
        yield return StartCoroutine(
            CountdownRoutine());

        NextWave();
    }

    // =========================================================
    // NEXT WAVE DELAY
    // =========================================================

    private IEnumerator NextWaveDelayRoutine()
    {
        yield return new WaitForSeconds(
            delayBetweenWaves);

        yield return StartCoroutine(
            CountdownRoutine());

        NextWave();
    }

    // =========================================================
    // COUNTDOWN
    // =========================================================

    private IEnumerator CountdownRoutine()
    {
        if (countdownTMP == null)
            yield break;

        countdownTMP.gameObject.SetActive(true);

        for (int i = countdownSeconds; i > 0; i--)
        {
            countdownTMP.text = i.ToString();

            yield return new WaitForSeconds(1f);
        }
        countdownTMP.text = $"¡INICIA LA OLEADA! {currentWaveIndex + 1}";

        yield return new WaitForSeconds(1f);

        countdownTMP.gameObject.SetActive(false);
    }

    // =========================================================
    // NEXT WAVE
    // =========================================================

    private void NextWave()
    {
        enemyAmountWave = 0;

        if (listWave == null || listWave.Length == 0)
            return;

        if (currentWaveIndex >= listWave.Length)
        {
            Debug.Log("ALL WAVES COMPLETED");

            if (countdownTMP != null)
            {
                countdownTMP.gameObject.SetActive(true);

                countdownTMP.text =
                    "¡TODAS LAS OLEADAS COMPLETADAS!";
            }

            return;
        }

        if (waveTMP != null)
        {
            waveTMP.text =
                $"Oleada: {currentWaveIndex + 1}";
        }

        // =====================================================
        // CREATE FIFO QUEUE
        // =====================================================

        enemyQueue = new Queue<GameObject>(
            listWave[currentWaveIndex].enemiesInWave);

        waitEnemySpawn = new WaitForSeconds(
            listWave[currentWaveIndex].spawnInterval);

        enemyAmountWave = enemyQueue.Count;

        // =====================================================
        // BUILD UI SNAPSHOT
        // =====================================================

        List<string> queueSnapshot =
            BuildQueueSnapshot();

        OnQueueCreated?.Invoke(
            queueSnapshot,
            currentWaveIndex,
            enemyAmountWave);

        StartCoroutine(SpawnRoutine());

        currentWaveIndex++;
    }

    // =========================================================
    // SPAWN ROUTINE
    // =========================================================

    private IEnumerator SpawnRoutine()
    {
        while (enemyQueue.Count > 0)
        {
            GameObject nextEnemy =
                enemyQueue.Dequeue();

            enemyIdCounter++;

            string cleanName =
                nextEnemy.name.Replace(
                    "(Clone)",
                    "");

            // =====================================================
            // UPDATE UI FIFO
            // =====================================================

            OnEnemyDequeued?.Invoke();

            // =====================================================
            // PLAY SPAWN SOUND FIRST
            // =====================================================

            if (spawnClip != null &&
                audioSource != null)
            {
                audioSource.PlayOneShot(
                    spawnClip);

                yield return new WaitForSeconds(
                    spawnClip.length - 0.5f);
            }

            // =====================================================
            // SPAWN ENEMY AFTER SOUND
            // =====================================================

            GameObject spawnedEnemy =
                LeanPool.Spawn(
                    nextEnemy,
                    spawnPoint.position,
                    spawnPoint.rotation);

            EnemyController enemyController =
                spawnedEnemy.GetComponent<EnemyController>();

            enemyController.Initialize(
                enemyIdCounter,
                cleanName);

            // =====================================================
            // TRACK ACTIVE ENEMIES
            // =====================================================

            currentAliveEnemies++;

            enemyController.OnEnemyRemoved +=
                HandleEnemyRemoved;

            // =====================================================
            // WAIT BETWEEN ENEMIES
            // =====================================================

            yield return waitEnemySpawn;
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
        currentAliveEnemies--;

        enemy.OnEnemyRemoved -=
            HandleEnemyRemoved;

        CheckWaveCompleted();
    }

    // =========================================================
    // CHECK WAVE COMPLETED
    // =========================================================

    private void CheckWaveCompleted()
    {
        bool queueEmpty =
            enemyQueue == null ||
            enemyQueue.Count == 0;

        bool noEnemiesAlive =
            currentAliveEnemies <= 0;

        if (queueEmpty && noEnemiesAlive)
        {
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

        if (enemyQueue == null)
            return snapshot;

        int previewId = enemyIdCounter;

        foreach (GameObject enemy in enemyQueue)
        {
            previewId++;

            string cleanName =
                enemy.name.Replace(
                    "(Clone)",
                    "");

            string displayName =
                $"{cleanName}_{previewId}";

            snapshot.Add(displayName);
        }

        return snapshot;
    }

    // =========================================================
    // DESTROY
    // =========================================================

    private void OnDestroy()
    {
        if (enemyQueue != null)
        {
            enemyQueue.Clear();
        }
    }
}