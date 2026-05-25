using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyData", menuName = "Scriptable/EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    public EnemyType enemyType;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 4f;
    public float toleranceDistance = 0.1f;

    [Header("Rewards")]
    public int goldReward;
    public float damageToCore;

    [Header("Combat")]
    public float attackResistance;

    [Header("Audio")]
    public AudioClip deathSFX;

    [Header("FX")]
    public GameObject deathFX;
}
