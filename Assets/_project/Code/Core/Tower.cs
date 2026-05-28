using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private TowerDataSO towerDataSO;
    

    private Node currentNode;

    // =========================================================
    // IDENTITY
    // =========================================================

    public int TowerId { get; private set; }

    public string DisplayName { get; private set; }

    // =========================================================
    // REFERENCES
    // =========================================================

    public TowerTargeting TowerTargeting { get; private set; }

    public Node CurrentNode
    {
        get => currentNode;
        set => currentNode = value;
    }

    public TowerDataSO TowerDataSo => towerDataSO;

    private void Awake()
    {
        TowerTargeting =
            GetComponentInChildren<TowerTargeting>();
    }

    // =========================================================
    // INITIALIZE
    // =========================================================

    public void Initialize(
        int id,
        string towerBaseName)
    {
        TowerId = id;

        DisplayName =
            $"{towerBaseName}_{id}";

        // =========================================================
        // ROOT NAME
        // =========================================================

        gameObject.name = DisplayName;

        // =========================================================
        // SYNC TARGETING
        // =========================================================

        if (TowerTargeting != null)
        {
            TowerTargeting.SetOwnerTower(this);
        }
    }
}