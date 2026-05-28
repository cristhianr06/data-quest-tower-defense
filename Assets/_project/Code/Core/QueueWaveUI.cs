using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueueWaveUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject queueWavePanel;

    [Header("Buttons")]
    [SerializeField] private Button showQueueWaveButton;
    [SerializeField] private Button closeButton;

    [Header("Content")]
    [SerializeField] private Transform contentParent;

    [SerializeField] private TextMeshProUGUI enemyTextPrefab;
    [SerializeField] private TextMeshProUGUI currentWaveText;

    // =========================================================
    // FIFO VISUAL QUEUE
    // =========================================================

    private readonly List<TextMeshProUGUI> _spawnedTexts =
        new();

    private void Awake()
    {
        showQueueWaveButton.onClick.AddListener(OpenPanel);

        closeButton.onClick.AddListener(ClosePanel);
    }

    private void Start()
    {
        if (SpawnManager.Instance == null)
            return;

        SpawnManager.Instance.OnQueueCreated += HandleQueueCreated;

        SpawnManager.Instance.OnEnemyDequeued += HandleEnemyDequeued;

        SpawnManager.Instance.OnQueueCleared += HandleQueueCleared;
    }

    private void OnDestroy()
    {
        if (SpawnManager.Instance == null)
            return;

        SpawnManager.Instance.OnQueueCreated -= HandleQueueCreated;

        SpawnManager.Instance.OnEnemyDequeued -= HandleEnemyDequeued;

        SpawnManager.Instance.OnQueueCleared -= HandleQueueCleared;
    }

    // =========================================================
    // PANEL
    // =========================================================

    private void OpenPanel()
    {
        queueWavePanel.SetActive(true);

        RefreshFullQueueUI();
    }

    private void ClosePanel()
    {
        queueWavePanel.SetActive(false);

        ClearUI();
    }

    // =========================================================
    // EVENTS
    // =========================================================

    private void HandleQueueCreated(List<string> queueNames, int waveIndex, int enemyAmount)
    {
        if (!queueWavePanel.activeSelf)
            return;
        currentWaveText.text = $"{enemyAmount} enemigos en cola para  la Oleada {waveIndex + 1}";
        BuildQueueUI(queueNames);
    }

    private void HandleEnemyDequeued()
    {
        if (!queueWavePanel.activeSelf)
            return;

        RemoveFirstEnemyUI();
    }

    private void HandleQueueCleared()
    {
        ClearUI();
    }

    // =========================================================
    // UI BUILD
    // =========================================================

    private void RefreshFullQueueUI()
    {
        ClearUI();

        List<string> snapshot =
            SpawnManager.Instance.GetCurrentQueueSnapshot();

        BuildQueueUI(snapshot);
    }

    private void BuildQueueUI(List<string> queueNames)
    {
        ClearUI();

        foreach (string enemyName in queueNames)
        {
            CreateEnemyText(enemyName);
        }
    }

    private void CreateEnemyText(string enemyName)
    {
        TextMeshProUGUI textInstance =
            Instantiate(
                enemyTextPrefab,
                contentParent);

        textInstance.text = enemyName;

        _spawnedTexts.Add(textInstance);
    }

    // =========================================================
    // FIFO REMOVE
    // =========================================================

    private void RemoveFirstEnemyUI()
    {
        if (_spawnedTexts.Count == 0)
            return;

        TextMeshProUGUI firstItem =
            _spawnedTexts[0];

        _spawnedTexts.RemoveAt(0);

        Destroy(firstItem.gameObject);
    }

    // =========================================================
    // CLEANUP
    // =========================================================

    private void ClearUI()
    {
        for (int i = 0; i < _spawnedTexts.Count; i++)
        {
            if (_spawnedTexts[i] != null)
            {
                Destroy(_spawnedTexts[i].gameObject);
            }
        }

        _spawnedTexts.Clear();
    }
}