using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wave", menuName = "Scriptable/Wave")]
public class WaveSO : ScriptableObject
{
    public List<GameObject> enemiesInWave;
    public float spawnInterval = 1.0f;
}
