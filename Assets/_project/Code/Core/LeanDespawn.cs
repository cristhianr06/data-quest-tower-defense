using System.Collections;
using Lean.Pool;
using UnityEngine;

public class LeanDespawn : MonoBehaviour, IPoolable
{
    [SerializeField] private float delayDespawn = 1.0f;
    
    private WaitForSeconds _wait;
    private Coroutine _despawnCoroutine;

    private void Awake()
    {
        _wait = new WaitForSeconds(delayDespawn);
    }


    public void OnSpawn()
    {
        if (_despawnCoroutine != null)
        {
            StopCoroutine(_despawnCoroutine);
            _despawnCoroutine = null;
        }

        _despawnCoroutine = StartCoroutine(DespawnRoutine());
    }

    public void OnDespawn()
    {
       
    }
    
    private IEnumerator DespawnRoutine()
    {
        yield return _wait;
        LeanPool.Despawn(this);
    }
}
