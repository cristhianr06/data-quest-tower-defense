using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DictionaryEnemyUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField]
    private GameObject dictionaryPanel;

    [Header("Buttons")]
    [SerializeField]
    private Button openButton;

    [SerializeField]
    private Button closeButton;

    [Header("Runtime Lookup")]
    [SerializeField]
    private TextMeshProUGUI runtimeLookupTMP;

    private bool isSubscribed;

    // =========================================================
    // UNITY
    // =========================================================

    private void Awake()
    {
        openButton.onClick.AddListener(
            OpenPanel);

        closeButton.onClick.AddListener(
            ClosePanel);
    }

    private void Start()
    {
        SubscribeToDatabase();
    }

    private void OnDestroy()
    {
        UnsubscribeFromDatabase();
    }

    // =========================================================
    // SUBSCRIBE
    // =========================================================

    private void SubscribeToDatabase()
    {
        if (isSubscribed)
            return;

        if (EnemyDatabase.Instance == null)
            return;

        EnemyDatabase.Instance.OnDictionaryLookup +=
            HandleLookup;

        isSubscribed = true;
    }

    private void UnsubscribeFromDatabase()
    {
        if (!isSubscribed)
            return;

        if (EnemyDatabase.Instance == null)
            return;

        EnemyDatabase.Instance.OnDictionaryLookup -=
            HandleLookup;

        isSubscribed = false;
    }

    // =========================================================
    // PANEL
    // =========================================================

    private void OpenPanel()
    {
        dictionaryPanel.SetActive(true);

        RefreshLastLookup();
    }

    private void ClosePanel()
    {
        dictionaryPanel.SetActive(false);

        runtimeLookupTMP.text = "";
    }

    // =========================================================
    // REFRESH
    // =========================================================

    private void RefreshLastLookup()
    {
        if (EnemyDatabase.Instance == null)
            return;

        if (EnemyDatabase.Instance.LastLookupData == null)
        {
            runtimeLookupTMP.text =
                "Esperando consultas al diccionario...";
                
            return;
        }

        UpdateLookupUI(
            EnemyDatabase.Instance.LastLookupType,
            EnemyDatabase.Instance.LastLookupData);
    }

    // =========================================================
    // REALTIME LOOKUP
    // =========================================================

    private void HandleLookup(
        EnemyType enemyType,
        EnemyDataSO enemyData)
    {
        if (!dictionaryPanel.activeSelf)
            return;

        UpdateLookupUI(
            enemyType,
            enemyData);
    }

    // =========================================================
    // UI
    // =========================================================

    private void UpdateLookupUI(
        EnemyType enemyType,
        EnemyDataSO enemyData)
    {
        runtimeLookupTMP.text =
            $"Tipo de enemigo (KEY): {enemyType}\n" +
            $"Datos encontrados:\n" +
            $"Vida: {enemyData.maxHealth}\n" +
            $"Velocidad: {enemyData.moveSpeed} m/s\n" +
            $"Oro: {enemyData.goldReward}";
    }
}