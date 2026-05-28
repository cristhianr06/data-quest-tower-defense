using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerTargetUI : MonoBehaviour
{
    public static TowerTargetUI Instance { get; private set; }
    [SerializeField] private GameObject listPanel;
    [SerializeField] private Transform content;
    [SerializeField] private TextMeshProUGUI nameTowerText;
    [SerializeField] private TextMeshProUGUI textPrefab;
    [SerializeField] private Button _closeButton;

    private Dictionary<int, TMP_Text> activeTexts = new();
    public event Action<TowerTargeting> OnTowerSelected;
    public TowerTargeting SelectedTower { get; private set; }
    private TowerTargeting _currentListeningTower;
    private GameObject _currentTowerArea;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        listPanel.SetActive(false);
    }
    private void OnEnable()
    {
        OnTowerSelected += HandleTowerSelected;
        _closeButton.onClick.AddListener(ClosePanel);
    }
    private void OnDisable()
    {
        OnTowerSelected -= HandleTowerSelected;
        _closeButton.onClick.RemoveListener(ClosePanel);
    }
    public void SelectTower(TowerTargeting towerTargeting)
    {
        if (SelectedTower == towerTargeting)
            return;

        CleanupCurrentTower();

        SelectedTower = towerTargeting;

        _currentTowerArea = towerTargeting.TowerArea;

        _currentTowerArea.SetActive(true);

        nameTowerText.text = $"Lista de objetivos en {towerTargeting.OwnerTower.DisplayName}";

        OnTowerSelected?.Invoke(SelectedTower);

        if (!listPanel.activeSelf)
        {
            listPanel.SetActive(true);
        }
    }
    private void CleanupCurrentTower()
    {
        ClearUI();

        if (_currentListeningTower != null)
        {
            _currentListeningTower.OnEnemyAdded -= HandleEnemyAdded;

            _currentListeningTower.OnEnemyRemoved -= HandleEnemyRemoved;
            
            _currentListeningTower = null;
        }
        if (_currentTowerArea != null)
        {
            _currentTowerArea.SetActive(false);
            _currentTowerArea = null;
        }
    }
    private void ClosePanel()
    {
        CleanupCurrentTower();

        SelectedTower = null;

        listPanel.SetActive(false);
    }
    private void HandleTowerSelected(TowerTargeting tower)
    {
        _currentListeningTower = tower;

        _currentListeningTower.OnEnemyAdded += HandleEnemyAdded;

        _currentListeningTower.OnEnemyRemoved += HandleEnemyRemoved;

        foreach (EnemyHealth enemy in tower.enemiesInRange)
        {
            HandleEnemyAdded(enemy);
        }
    }
    private void HandleEnemyAdded(EnemyHealth enemy)
    {
        AddEnemy(enemy.GetComponent<EnemyController>());
    }

    private void HandleEnemyRemoved(EnemyHealth enemy)
    {
        RemoveEnemy(enemy.GetComponent<EnemyController>());
    }
    public void AddEnemy(EnemyController enemyController)
    {
        if (activeTexts.ContainsKey(enemyController.EnemyId))
            return;

        TextMeshProUGUI textInstance = Instantiate(textPrefab, content);

        textInstance.text = enemyController.DisplayName;

        activeTexts.Add(enemyController.EnemyId, textInstance);
    }

    public void RemoveEnemy(EnemyController enemyController)
    {
        if (!activeTexts.ContainsKey(enemyController.EnemyId))
            return;

        Destroy(activeTexts[enemyController.EnemyId].gameObject);

        activeTexts.Remove(enemyController.EnemyId);
    }
    private void ClearUI()
    {
        foreach (var item in activeTexts)
        {
            Destroy(item.Value.gameObject);
        }

        activeTexts.Clear();
    }
}