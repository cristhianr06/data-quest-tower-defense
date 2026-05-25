using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacementTowerManager : MonoBehaviour
{
    public GameObject menuTower;
    public TowerDataSO crossbowData;
    public TowerDataSO cannonData;
    public static PlacementTowerManager Instance;
    public Button crossbowButton;
    public Button cannonButton;
    public GameObject crossbowPrefab;
    public GameObject cannonPrefab;
    public List<Node> allNodes = new();
    private Node currentSelectedNode;
    private Transform _nodeTransform;
    
    
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        crossbowButton.onClick.AddListener(CreateCrossbowTower);
        cannonButton.onClick.AddListener(CreateCannonTower);
        Node.OnNodeSelected += HandleNodeSelected;
    }

    private void OnDisable()
    {
        crossbowButton.onClick.RemoveListener(CreateCrossbowTower);
        cannonButton.onClick.RemoveListener(CreateCannonTower);
        Node.OnNodeSelected -= HandleNodeSelected;
    }
    
    private void HandleNodeSelected(Node node, Transform nodeTransform)
    {
        if (currentSelectedNode != null)
        {
            currentSelectedNode.DeselectedNode();
        }
        currentSelectedNode = node;
        _nodeTransform = nodeTransform;
    }

    private void CreateCrossbowTower()
    {
        CreateTower(crossbowPrefab, crossbowData.cost);
        
    }

    private void CreateCannonTower()
    {
        CreateTower(cannonPrefab, cannonData.cost);
        
    }
    private void CreateTower(GameObject towerPrefab, int cost)
    {
        if (currentSelectedNode == null)
        {
            Debug.Log($"currentSelectedNode is null");
            return; 
        }

        if (currentSelectedNode.IsOccupied)
        {
            Debug.Log("Nodo ocupado");
            return;
        }
        
        if (PlayerEconomy.Instance.SpendGold(cost))
        {
            Debug.Log("Creating Tower");
            Instantiate(towerPrefab, _nodeTransform.position, Quaternion.identity);
            menuTower.SetActive(false);
            currentSelectedNode.DisableVfx();
        }
        else
        {
            Debug.Log($"No se pudo crear la torre");
        }
    }
}
