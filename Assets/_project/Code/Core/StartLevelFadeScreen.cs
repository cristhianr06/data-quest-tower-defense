using PrimeTween;
using UnityEngine;

public class StartLevelFadeScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup _backgroundOn;
    [SerializeField] private float _animDuration;
    void Start()
    {
        UpdateFade();
    }

    private void UpdateFade()
    {
        Tween.Alpha(
            _backgroundOn, 
            1, 
            0, 
            _animDuration,
            Ease.InBack);
    }
}
