using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StackTowersUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject stackPanel;

    [Header("Buttons")]
    [SerializeField] private Button openButton;

    [SerializeField] private Button closeButton;

    [Header("Content")]
    [SerializeField] private Transform contentParent;

    [SerializeField] private TextMeshProUGUI towerTextPrefab;

    // =========================================================
    // VISUAL STACK
    // =========================================================

    private readonly List<TextMeshProUGUI> _spawnedTexts =
        new();

    private void Awake()
    {
        openButton.onClick.AddListener(OpenPanel);

        closeButton.onClick.AddListener(ClosePanel);
    }

    private void Start()
    {
        if (PlacementTowerManager.Instance == null)
            return;

        PlacementTowerManager.Instance.OnStackCreated +=
            HandleStackCreated;

        PlacementTowerManager.Instance.OnTowerPushed +=
            HandleTowerPushed;

        PlacementTowerManager.Instance.OnTowerPopped +=
            HandleTowerPopped;
    }

    private void OnDestroy()
    {
        if (PlacementTowerManager.Instance == null)
            return;

        PlacementTowerManager.Instance.OnStackCreated -=
            HandleStackCreated;

        PlacementTowerManager.Instance.OnTowerPushed -=
            HandleTowerPushed;

        PlacementTowerManager.Instance.OnTowerPopped -=
            HandleTowerPopped;
    }

    // =========================================================
    // PANEL
    // =========================================================

    private void OpenPanel()
    {
        stackPanel.SetActive(true);

        RefreshStackUI();
    }

    private void ClosePanel()
    {
        stackPanel.SetActive(false);

        ClearUI();
    }

    // =========================================================
    // EVENTS
    // =========================================================

    private void HandleStackCreated(
        List<string> towerNames)
    {
        if (!stackPanel.activeSelf)
            return;

        BuildStackUI(towerNames);
    }

    private void HandleTowerPushed(
        string towerName)
    {
        if (!stackPanel.activeSelf)
            return;

        AddTowerToTop(towerName);
    }

    private void HandleTowerPopped()
    {
        if (!stackPanel.activeSelf)
            return;

        RemoveTowerFromTop();
    }

    // =========================================================
    // BUILD UI
    // =========================================================

    private void RefreshStackUI()
    {
        ClearUI();

        List<string> snapshot =
            PlacementTowerManager.Instance
                .GetStackSnapshot();

        BuildStackUI(snapshot);
    }

    private void BuildStackUI(
        List<string> towerNames)
    {
        ClearUI();

        foreach (string towerName in towerNames)
        {
            CreateTowerText(towerName);
        }
    }

    private void CreateTowerText(
        string towerName)
    {
        TextMeshProUGUI textInstance =
            Instantiate(
                towerTextPrefab,
                contentParent);

        textInstance.text = towerName;

        // =========================================================
        // TOP OF STACK VISUAL
        // =========================================================

        textInstance.transform.SetAsFirstSibling();

        _spawnedTexts.Insert(0, textInstance);
    }

    // =========================================================
    // PUSH (LIFO)
    // =========================================================

    private void AddTowerToTop(
        string towerName)
    {
        CreateTowerText(towerName);

        //Debug.Log($"UI PUSH -> {towerName}");
    }

    // =========================================================
    // POP (LIFO)
    // =========================================================

    private void RemoveTowerFromTop()
    {
        if (_spawnedTexts.Count == 0)
            return;

        // =========================================================
        // REMOVE TOP
        // =========================================================

        TextMeshProUGUI topItem =
            _spawnedTexts[0];

        _spawnedTexts.RemoveAt(0);

        Destroy(topItem.gameObject);

        //Debug.Log("UI POP -> TOP REMOVED");
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
                Destroy(
                    _spawnedTexts[i].gameObject);
            }
        }

        _spawnedTexts.Clear();
    }
}