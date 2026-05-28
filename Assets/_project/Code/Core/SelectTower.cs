using UnityEngine;
using UnityEngine.EventSystems;

public class SelectTower : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private AudioClip _selectSound;
    private TowerTargeting _towerTargeting;
    private AudioSource _audioSource;
    private void Awake()
    {
        _towerTargeting = GetComponentInChildren<TowerTargeting>();
        _audioSource = GetComponentInChildren<AudioSource>();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (_towerTargeting != null)
        {
            _audioSource.PlayOneShot(_selectSound);
            TowerTargetUI.Instance.SelectTower(_towerTargeting);
        }
    }
}
