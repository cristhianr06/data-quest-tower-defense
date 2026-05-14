using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/TowerData", fileName ="new toweData")]
public class TowerDataSO : ScriptableObject
{
    public string towerName;

    public float damage;

    public float fireRate;

    public float range;

    public int cost;

    public GameObject projectilePrefab;

    public AudioClip shootSFX;
}