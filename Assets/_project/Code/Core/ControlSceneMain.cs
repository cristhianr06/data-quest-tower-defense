using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlSceneMain
    : MonoBehaviour
{
    [Header("Timing")]

    [SerializeField]
    private float startDelay = 1f;

    [SerializeField]
    private float totalLoadTime = 5f;

    [SerializeField]
    [Range(0.6f, 0.95f)]
    private float fadeInThreshold = 0.85f;

    [Header("UI")]

    [SerializeField]
    private Slider loadingSlider;

    [SerializeField]
    private TextMeshProUGUI percentageTMP;

    [Header("References")]

    [SerializeField]
    private FadeScreen fadeScreen;

    private ManagerScene managerScene;

    private bool hasStartedFadeIn;

    // =====================================================
    // UNITY
    // =====================================================

    private void Awake()
    {
        managerScene =
            GetComponent<ManagerScene>();

        loadingSlider.value = 0f;

        percentageTMP.text = "0%";
    }

    private void Start()
    {
        StartCoroutine(
            LoadSequenceRoutine());
    }

    // =====================================================
    // MAIN SEQUENCE
    // =====================================================

    private IEnumerator LoadSequenceRoutine()
    {
        // =========================================
        // INITIAL WAIT
        // =========================================

        yield return new WaitForSeconds(
            startDelay);

        // =========================================
        // START FADE OUT
        // =========================================

        fadeScreen.FadeOut();

        // WAIT FADE OUT

        yield return new WaitForSeconds(
            fadeScreen.FadeDuration);

        // =========================================
        // START LOADING
        // =========================================

        float elapsedTime = 0f;

        while (elapsedTime < totalLoadTime)
        {
            elapsedTime += Time.deltaTime;

            float progress =
                elapsedTime / totalLoadTime;

            progress =
                Mathf.Clamp01(progress);

            // =====================================
            // UPDATE UI
            // =====================================

            loadingSlider.value =
                progress;

            int percentage =
                Mathf.RoundToInt(
                    progress * 100f);

            percentageTMP.text =
                percentage + "%";

            // =====================================
            // START FINAL FADE IN
            // =====================================

            if (!hasStartedFadeIn &&
                progress >= fadeInThreshold)
            {
                hasStartedFadeIn = true;

                fadeScreen.FadeIn();
            }

            yield return null;
        }

        // =========================================
        // ENSURE FINAL VALUES
        // =========================================

        loadingSlider.value = 1f;

        percentageTMP.text = "100%";

        // WAIT FINAL FADE

        yield return new WaitForSeconds(
            fadeScreen.FadeDuration);

        // =========================================
        // NEXT SCENE
        // =========================================

        managerScene.NextScene();
    }
}