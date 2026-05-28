using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectEnemy : MonoBehaviour, IPointerDownHandler
{
    private EnemyMovement _enemyMovement;
    private EnemyController _enemyController;

    private void Awake()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        _enemyController = GetComponent<EnemyController>();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (_enemyMovement != null)
        {
            var nameEnemy = gameObject.name;
            SelectionManager.Instance.SelectEnemy(_enemyMovement, _enemyController);
        }
    }
}
