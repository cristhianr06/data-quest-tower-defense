using UnityEngine;
using UnityEngine.EventSystems;

public class SelectTower : MonoBehaviour, IPointerDownHandler
{
    private TowerTargeting _towerTargeting;
    private void Awake()
    {
        _towerTargeting = GetComponentInChildren<TowerTargeting>();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (_towerTargeting != null)
        {
            TowerTargetUI.Instance.SelectTower(_towerTargeting);
        }
    }
}
