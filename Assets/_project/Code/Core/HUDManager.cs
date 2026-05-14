using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    private void Start()
    {
        PlayerEconomy.Instance.OnGoldChanged += UpdateGold;
    }

    private void UpdateGold(int gold)
    {
        goldText.text = $"{gold}";
    }

    private void OnDestroy()
    {
        PlayerEconomy.Instance.OnGoldChanged -= UpdateGold;
    }
}