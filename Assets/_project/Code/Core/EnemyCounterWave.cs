using System;
using UnityEngine;

public class EnemyCounterWave : MonoBehaviour
{
    public static EnemyCounterWave Instance { get; private set; }
    public event Action endCurrentWave;
    public int amountCurrentWave;

    private void Awake()
    {
      if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
      Instance = this;
    }

    public void OnReduceEnemies(int counter)
    {
        amountCurrentWave -= counter;
        if(amountCurrentWave == 0)
        {
            endCurrentWave?.Invoke();
        }
    }
}
