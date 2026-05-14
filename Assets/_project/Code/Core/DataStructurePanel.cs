using TMPro;
using UnityEngine;

public class DataStructurePanel : MonoBehaviour
{
    public TextMeshProUGUI queueText;
    public TextMeshProUGUI stackText;
    public TextMeshProUGUI dictionaryText;

    private void Update()
    {
        UpdateQueue();

        UpdateStack();

        UpdateDictionary();
    }

    private void UpdateQueue()
    {
        queueText.text =
            $"QUEUE SPAWN\n" +
            $"En cola: {SpawnManager.Instance.CurrentQueueCount}";
    }

    private void UpdateStack()
    {
        stackText.text =
            $"STACK TORRES\n" +
            $"Historial: {BuildManager.Instance.TowerHistory.Count}";
    }

    private void UpdateDictionary()
    {
        dictionaryText.text = "DICTIONARY ENEMIES\n";

        foreach (var enemy in EnemyDatabase.Instance.EnemyDictionary)
        {
            dictionaryText.text +=
                $"{enemy.Key} -> HP:{enemy.Value.maxHealth}\n";
        }
    }
}