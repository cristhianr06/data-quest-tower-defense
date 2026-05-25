using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SelectNode : MonoBehaviour, IPointerDownHandler
{
    private Node _nodeScript;
    public UnityEvent OnSelect;

    private void Awake()
    {
        _nodeScript = GetComponent<Node>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnSelect?.Invoke();
            _nodeScript.SelectedNode();
        }
    }
}
