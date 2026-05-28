using System;
using UnityEngine;

public class WaypointsSignal : MonoBehaviour
{
    [SerializeField] private GameObject _arrayUiGameObject;
    [SerializeField] private GameObject[] _waypointsMesh;

    private bool _isEnabled;

    private void EnableMesh()
    {
        foreach (GameObject mesh in _waypointsMesh)
        {
            mesh.SetActive(true);
        }
    }

    private void DisableMesh()
    {
        foreach (GameObject mesh in _waypointsMesh)
        {
            mesh.SetActive(false);
        }
    }

    private void Update()
    {
        if (_arrayUiGameObject.activeSelf)
        {
            EnableMesh();
        }
        else
        {
            DisableMesh();
        }
    }
}
