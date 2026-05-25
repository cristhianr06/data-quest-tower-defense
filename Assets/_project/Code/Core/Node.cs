using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    public GameObject selectedVfx;
    public bool IsOccupied { get; private set; }
    public bool IsSelected { get; private set; }

    public static Action<Node, Transform> OnNodeSelected;
    
    public void SelectedNode()
    {
        IsSelected = true;
        OnNodeSelected?.Invoke(this,  transform);
        Debug.Log($"{gameObject.name} Selected");
    }

    public void DeselectedNode()
    {
        IsSelected = false;
        Debug.Log($"{gameObject.name} Deselected");
    }

    public void SetOccupied(bool value)
    {
        IsOccupied = value;
    }

    public void DisableVfx()
    {
        selectedVfx.SetActive(false);
    }
}