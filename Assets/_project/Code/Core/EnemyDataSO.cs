using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyData", menuName = "Scriptable/EnemyData")]
public class EnemyDataSO : ScriptableObject
{
    public float maxHealth = 100f;
    public float moveSpeed = 2f;
    public float toleranceDistance = 0.1f;
    public float rotationSpeed = 4f;
}
