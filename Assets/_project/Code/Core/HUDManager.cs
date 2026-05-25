using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    private void Start()
    {
        goldText.text = ("100");
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