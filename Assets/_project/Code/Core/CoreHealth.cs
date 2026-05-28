using System;
using UnityEngine;
using UnityEngine.UI;

public class CoreHealth : MonoBehaviour
{
    [SerializeField] private GameObject _damageParticlesPrefab;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _damageSound;
    public GameObject gameOverPanel;
    public static CoreHealth Instance;

    public float maxHealth = 100f;

    public float currentHealth;

    public Slider healthBar;

    public event Action OnGameOver;
    private ParticleSystem _damageParticle;

    private void Awake()
    {
        Instance = this;
        _damageParticlesPrefab.SetActive(false);
        _damageParticle = _damageParticlesPrefab.GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        healthBar.maxValue = maxHealth;
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        _audioSource.PlayOneShot(_damageSound);
        _damageParticlesPrefab.SetActive(true);
        _damageParticle.Play();
        UpdateUI();

        if (currentHealth <= 0.0f)
        {
            OnGameOver?.Invoke();
            gameOverPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void UpdateUI()
    {
        healthBar.value = currentHealth;
    }
}