using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIEnemyInspector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameEnemyTMP;
    [SerializeField] private GameObject[] waypointsImg;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private Button _closeButton;

    private EnemyMovement _trackedEnemy;

    public EnemyMovement SelectedEnemy { get; private set; }

    private GameObject _currentActiveImg;

    private GameObject _currentEnemyArea;

    private void Start()
    {
        SelectionManager.Instance.OnEnemySelected += HandleEnemySelected;

        _closeButton.onClick.AddListener(ClosePanel);

        infoPanel.SetActive(false);
    }

    private void OnDisable()
    {
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.OnEnemySelected -= HandleEnemySelected;
        }

        _closeButton.onClick.RemoveListener(ClosePanel);

        CleanupCurrentEnemy();
    }

    private void ClosePanel()
    {
        CleanupCurrentEnemy();

        SelectedEnemy = null;

        infoPanel.SetActive(false);

        // MUY IMPORTANTE
        if (SelectionManager.Instance != null)
        {
            SelectionManager.Instance.ClearSelection();
        }
    }

    private void CleanupCurrentEnemy()
    {
        if (_trackedEnemy != null)
        {
            _trackedEnemy.OnWaypointChanged -= UpdateWaypointUI;

            _trackedEnemy = null;
        }

        if (_currentActiveImg != null)
        {
            _currentActiveImg.SetActive(false);

            _currentActiveImg = null;
        }

        if (_currentEnemyArea != null)
        {
            _currentEnemyArea.SetActive(false);

            _currentEnemyArea = null;
        }
    }

    private void HandleEnemySelected(
        EnemyMovement newEnemy,
        EnemyController newController)
    {
        if (SelectedEnemy == newEnemy)
            return;

        CleanupCurrentEnemy();

        if (newEnemy == null || newController == null)
        {
            infoPanel.SetActive(false);
            return;
        }

        SelectedEnemy = newEnemy;

        _trackedEnemy = newEnemy;

        _currentEnemyArea = newController.enemyArea;

        if (_currentEnemyArea != null)
        {
            _currentEnemyArea.SetActive(true);
        }

        nameEnemyTMP.text =
            $"Puntos de ruta de enemigo {newController.DisplayName}";

        _trackedEnemy.OnWaypointChanged += UpdateWaypointUI;

        if (!infoPanel.activeSelf)
        {
            infoPanel.SetActive(true);
        }

        UpdateWaypointUI(_trackedEnemy.CurrentWaypointIndex);
    }

    private void UpdateWaypointUI(int newWaypointIndex)
    {
        int uiIndex = newWaypointIndex - 1;

        if (uiIndex < 0 || uiIndex >= waypointsImg.Length)
            return;

        if (_currentActiveImg != null)
        {
            _currentActiveImg.SetActive(false);
        }

        _currentActiveImg = waypointsImg[uiIndex];

        _currentActiveImg.SetActive(true);
    }
}