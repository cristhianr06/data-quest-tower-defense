using System.Collections.Generic;
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

    [Header("Dictionary Content")]
    [SerializeField]
    private Transform dictionaryContent;

    //[SerializeField] private TextMeshProUGUI dictionaryTextPrefab;

    [Header("Runtime Lookup")]
    [SerializeField]
    private TextMeshProUGUI runtimeLookupTMP;

    private readonly List<TextMeshProUGUI>
        spawnedTexts = new();

    private void Awake()
    {
        openButton.onClick.AddListener(
            OpenPanel);

        closeButton.onClick.AddListener(
            ClosePanel);
    }

    private void OnEnable()
    {
        if (EnemyDatabase.Instance == null)
            return;

        EnemyDatabase.Instance.OnDictionaryLookup +=
            HandleLookup;
    }

    private void OnDisable()
    {
        if (EnemyDatabase.Instance == null)
            return;

        EnemyDatabase.Instance.OnDictionaryLookup -=
            HandleLookup;
    }

    // =========================================================
    // PANEL
    // =========================================================

    private void OpenPanel()
    {
        dictionaryPanel.SetActive(true);

        //BuildDictionaryUI();
    }

    private void ClosePanel()
    {
        dictionaryPanel.SetActive(false);

        ClearUI();

        runtimeLookupTMP.text = "";
    }

    // =========================================================
    // BUILD STATIC DICTIONARY
    // =========================================================

    // private void BuildDictionaryUI()
    // {
    //     ClearUI();
    //
    //     IReadOnlyDictionary<EnemyType, EnemyDataSO>dictionary = EnemyDatabase.Instance.EnemyDictionary;
    //
    //     foreach (
    //         KeyValuePair<EnemyType, EnemyDataSO>
    //         pair in dictionary)
    //     {
    //         CreateDictionaryEntry(pair);
    //     }
    // }

    // private void CreateDictionaryEntry(
    //     KeyValuePair<EnemyType, EnemyDataSO>
    //     pair)
    // {
    //     TextMeshProUGUI textInstance =
    //         Instantiate(
    //             dictionaryTextPrefab,
    //             dictionaryContent);
    //
    //     EnemyDataSO data = pair.Value;
    //
    //     textInstance.text =
    //         $"KEY -> {pair.Key}\n" +
    //         $"VALUE\n" +
    //         $"Vida: {data.maxHealth}\n" +
    //         $"Velocidad: {data.moveSpeed} m/s\n" +
    //         $"Oro: {data.goldReward}";
    //
    //     spawnedTexts.Add(textInstance);
    // }

    // =========================================================
    // REAL-TIME LOOKUP
    // =========================================================

    private void HandleLookup(
        EnemyType enemyType,
        EnemyDataSO enemyData)
    {
        if (!dictionaryPanel.activeSelf)
            return;

        runtimeLookupTMP.text =
            $"Tipo de enemigo (KEY): {enemyType}\n" +
            $"Datos encontrados:\n" +
            $"Vida: {enemyData.maxHealth}\n" +
            $"Velocidad: {enemyData.moveSpeed} m/s\n" +
            $"Oro: {enemyData.goldReward}";
    }

    // =========================================================
    // CLEAR
    // =========================================================

    private void ClearUI()
    {
        for (int i = 0; i < spawnedTexts.Count; i++)
        {
            if (spawnedTexts[i] != null)
            {
                Destroy(
                    spawnedTexts[i].gameObject);
            }
        }

        spawnedTexts.Clear();
    }
}