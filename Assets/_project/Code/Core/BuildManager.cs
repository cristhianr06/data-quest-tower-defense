using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager Instance;

    public TowerDataSO selectedTower;

    private TowerNode _selectedNode;

    private Stack<GameObject> _towerHistory = new();

    public Stack<GameObject> TowerHistory => _towerHistory;

    private void Awake()
    {
        Instance = this;
    }

    public void SelectNode(TowerNode node)
    {
        _selectedNode = node;
    }

    public void BuildTower(GameObject towerPrefab)
    {
        if (_selectedNode == null)
            return;

        if (_selectedNode.isOccupied)
            return;

        if (!PlayerEconomy.Instance.SpendGold(selectedTower.cost))
            return;

        GameObject tower = Instantiate(
            towerPrefab,
            _selectedNode.transform.position,
            Quaternion.identity);

        _selectedNode.currentTower = tower;
        _selectedNode.isOccupied = true;

        _towerHistory.Push(tower);
    }

    public void UndoLastTower()
    {
        if (_towerHistory.Count == 0)
            return;

        GameObject lastTower = _towerHistory.Pop();

        TowerNode[] nodes = FindObjectsByType<TowerNode>(
            FindObjectsSortMode.None);

        foreach (TowerNode node in nodes)
        {
            if (node.currentTower == lastTower)
            {
                node.currentTower = null;
                node.isOccupied = false;
                break;
            }
        }

        Destroy(lastTower);
    }
}