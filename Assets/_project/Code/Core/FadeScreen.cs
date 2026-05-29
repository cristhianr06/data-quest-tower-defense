using PrimeTween;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{
    [Header("Canvas Groups")]

    [SerializeField]
    private CanvasGroup backgroundGroup;

    [SerializeField]
    private CanvasGroup contentGroup;

    [Header("Animation")]

    [SerializeField]
    private float fadeDuration = 1f;

    [SerializeField]
    private Ease fadeEase =
        Ease.InOutQuad;

    // =====================================================
    // UNITY
    // =====================================================

    private void Awake()
    {
        // START BLACK

        backgroundGroup.alpha = 1f;

        contentGroup.alpha = 1f;
    }

    // =====================================================
    // FADE OUT
    // BLACK -> TRANSPARENT
    // =====================================================

    public void FadeOut()
    {
        Tween.Alpha(
            backgroundGroup,
            1f,
            0f,
            fadeDuration,
            fadeEase);
        
        Tween.Scale(
            contentGroup.transform,
            Vector3.one,
            fadeDuration,
            fadeEase);
    }

    // =====================================================
    // FADE IN
    // TRANSPARENT -> BLACK
    // =====================================================

    public void FadeIn()
    {
        Tween.Alpha(
            backgroundGroup,
            0f,
            1f,
            fadeDuration,
            fadeEase);
    }

    // =====================================================
    // GET DURATION
    // =====================================================

    public float FadeDuration =>
        fadeDuration;
}