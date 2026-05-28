using System;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; private set; }

    public event Action<EnemyMovement, EnemyController> OnEnemySelected;

    public EnemyMovement SelectedEnemy { get; private set; }

    public EnemyController ControllerEnemy { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SelectEnemy(EnemyMovement enemy, EnemyController controller)
    {
        // Evitar selección inválida
        if (enemy == null || controller == null)
            return;

        // Si es el mismo enemigo ya seleccionado
        if (SelectedEnemy == enemy)
        {
            // Si el panel está cerrado, reenviamos evento
            OnEnemySelected?.Invoke(enemy, controller);
            return;
        }

        SelectedEnemy = enemy;

        ControllerEnemy = controller;

        OnEnemySelected?.Invoke(
            SelectedEnemy,
            ControllerEnemy);
    }

    public void ClearSelection()
    {
        SelectedEnemy = null;

        ControllerEnemy = null;

        OnEnemySelected?.Invoke(null, null);
    }
}