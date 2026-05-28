using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementTowerManager : MonoBehaviour
{
    public static PlacementTowerManager Instance;

    [Header("Menu")]
    public GameObject menuTower;

    [Header("Tower Data")]
    public TowerDataSO crossbowData;
    public TowerDataSO cannonData;

    [Header("Buttons")]
    public Button crossbowButton;
    public Button cannonButton;
    public Button removeTowerButton;

    [Header("Prefabs")]
    public GameObject crossbowPrefab;
    public GameObject cannonPrefab;

    private Node currentSelectedNode;
    private Transform currentNodeTransform;

    private readonly Stack<GameObject> towerStack = new();

    private int _towerIdCounter = 0;

    // =========================================================
    // EVENTS
    // =========================================================

    /// <summary>
    /// Snapshot completo del stack.
    /// </summary>
    public event Action<List<string>> OnStackCreated;

    /// <summary>
    /// Nueva torre agregada al TOP.
    /// </summary>
    public event Action<string> OnTowerPushed;

    /// <summary>
    /// Torre removida del TOP.
    /// </summary>
    public event Action OnTowerPopped;

    public int StackCount => towerStack.Count;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        crossbowButton.onClick.AddListener(CreateCrossbowTower);

        cannonButton.onClick.AddListener(CreateCannonTower);

        removeTowerButton.onClick.AddListener(RemoveTower);

        Node.OnNodeSelected += HandleNodeSelected;
    }

    private void OnDisable()
    {
        crossbowButton.onClick.RemoveListener(CreateCrossbowTower);

        cannonButton.onClick.RemoveListener(CreateCannonTower);

        removeTowerButton.onClick.RemoveListener(RemoveTower);

        Node.OnNodeSelected -= HandleNodeSelected;
    }

    private void Start()
    {
        removeTowerButton.gameObject.SetActive(
            towerStack.Count > 0);

        BuildStackSnapshot();
    }

    private void HandleNodeSelected(
        Node node,
        Transform nodeTransform)
    {
        if (currentSelectedNode != null)
        {
            currentSelectedNode.DeselectedNode();
        }

        currentSelectedNode = node;

        currentNodeTransform = nodeTransform;
    }

    private void CreateCrossbowTower()
    {
        CreateTower(
            crossbowPrefab,
            crossbowData.cost);
    }

    private void CreateCannonTower()
    {
        CreateTower(
            cannonPrefab,
            cannonData.cost);
    }

    private void CreateTower(
        GameObject towerPrefab,
        int cost)
    {
        if (currentSelectedNode == null)
        {
            //Debug.Log("Current node is null");
            return;
        }

        if (currentSelectedNode.IsOccupied)
        {
            //Debug.Log("Node occupied");
            return;
        }

        if (!PlayerEconomy.Instance.SpendGold(cost))
        {
            //Debug.Log("Not enough gold");
            return;
        }

        // =========================================================
        // CREATE TOWER
        // =========================================================

        GameObject currentTower =
            Instantiate(
                towerPrefab,
                currentNodeTransform.position,
                Quaternion.identity);

        Tower tower =
            currentTower.GetComponent<Tower>();

        _towerIdCounter++;

        string cleanName =
            towerPrefab.name.Replace("(Clone)", "");

        // =========================================================
        // INITIALIZE ROOT TOWER
        // =========================================================

        tower.Initialize(
            _towerIdCounter,
            cleanName);

        tower.CurrentNode =
            currentSelectedNode;

        // =========================================================
        // PUSH STACK (LIFO)
        // =========================================================

        towerStack.Push(currentTower);

        OnTowerPushed?.Invoke(
            tower.DisplayName);

        removeTowerButton.gameObject.SetActive(
            towerStack.Count > 0);

        currentSelectedNode.SetOccupied(true);

        currentSelectedNode.DisableVfx();

        menuTower.SetActive(false);

        //Debug.Log($"PUSH -> {tower.DisplayName}");
    }

    private void RemoveTower()
    {
        if (towerStack.Count <= 0)
            return;

        // =========================================================
        // POP STACK (LIFO)
        // =========================================================

        GameObject lastTower =
            towerStack.Pop();

        Tower tower =
            lastTower.GetComponent<Tower>();

        tower.CurrentNode.SetOccupied(false);

        PlayerEconomy.Instance.AddGold(
            tower.TowerDataSo.cost / 2);

        removeTowerButton.gameObject.SetActive(
            towerStack.Count > 0);

        OnTowerPopped?.Invoke();

        //Debug.Log($"POP -> {lastTower.name}");

        Destroy(lastTower);
    }

    // =========================================================
    // SNAPSHOT FOR UI REBUILD
    // =========================================================

    public List<string> GetStackSnapshot()
    {
        List<string> snapshot = new();

        foreach (GameObject tower in towerStack)
        {
            snapshot.Add(tower.name);
        }

        return snapshot;
    }

    private void BuildStackSnapshot()
    {
        OnStackCreated?.Invoke(GetStackSnapshot());
    }
}