using UnityEngine;

public class WireClickProxy : MonoBehaviour
{
    private WireCollider owner;

    public void Initialize(WireCollider clickTarget)
    {
        owner = clickTarget;
    }

    private void OnMouseDown()
    {
        owner?.HandleWireClicked();
    }
}