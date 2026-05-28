using System;
using UnityEngine;

public class Node : MonoBehaviour
{
    //[SerializeField] private bool _isSelected;
    [SerializeField] private bool _isOccupied;
    public GameObject selectedVfx;
    

    public static Action<Node, Transform> OnNodeSelected;
    
    public bool IsOccupied => _isOccupied;

    public void SelectedNode()
    {
        //_isSelected = true;
        OnNodeSelected?.Invoke(this,  transform);
        //Debug.Log($"{gameObject.name} Selected");
    }

    public void DeselectedNode()
    {
        //_isSelected = false;
        //Debug.Log($"{gameObject.name} Deselected");
    }

    public void SetOccupied(bool value)
    {
        _isOccupied = value;
    }

    public void DisableVfx()
    {
        selectedVfx.SetActive(false);
    }
}