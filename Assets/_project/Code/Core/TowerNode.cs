using UnityEngine;

public class TowerNode : MonoBehaviour
{
    public bool isOccupied;

    public GameObject currentTower;

    public Renderer meshRenderer;

    public Material normalMat;
    public Material selectedMat;

    private void OnMouseDown()
    {
        BuildManager.Instance.SelectNode(this);
    }

    public void SetSelected(bool value)
    {
        meshRenderer.material = value ? selectedMat : normalMat;
    }
}