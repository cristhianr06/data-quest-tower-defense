using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectEnemy : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private AudioClip _selectSound;
    private EnemyMovement _enemyMovement;
    private EnemyController _enemyController;
    private AudioSource _audioSource;

    private void Awake()
    {
        _enemyMovement = GetComponent<EnemyMovement>();
        _enemyController = GetComponent<EnemyController>();
        _audioSource = GetComponent<AudioSource>();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (_enemyMovement != null)
        {
            _audioSource.PlayOneShot(_selectSound);
            SelectionManager.Instance.SelectEnemy(_enemyMovement, _enemyController);
        }
    }
}
