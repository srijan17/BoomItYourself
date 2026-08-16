using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class MovableWireObject : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private bool canMove = true;

    [Header("HighLight")]
    [SerializeField] private GameObject highlightObject;

    [Tooltip("The object that should move. Uses this transform if empty.")]
    [SerializeField] private Transform moveRoot;

    [Header("Events")]
    public UnityEvent onPickedUp;
    public UnityEvent onPlaced;
    public UnityEvent onMoveCancelled;


    [Header("Movement Bounds")]
    [SerializeField]
    private Vector2 boundsPadding = new Vector2(0.5f, 0.5f);
    public Vector2 BoundsPadding => boundsPadding;


    public bool CanMove => canMove;

    public Transform MoveRoot =>
        moveRoot != null ? moveRoot : transform;

    public void NotifyPickedUp()
    {
        onPickedUp?.Invoke();
    }

    public void NotifyPlaced()
    {
        onPlaced?.Invoke();
    }

    public void NotifyMoveCancelled()
    {
        onMoveCancelled?.Invoke();
    }

    public void Highlight()
    {
        if (highlightObject != null)
            highlightObject.SetActive(true);
    }

    public void Unhighlight()
    {
        if (highlightObject != null)
            highlightObject.SetActive(false);
    }
}