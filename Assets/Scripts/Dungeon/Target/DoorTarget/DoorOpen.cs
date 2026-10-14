using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    [SerializeField] private Vector3 initialRotation;
    [SerializeField] private Vector3 finalRotation;
    [SerializeField] private float animationTime = 0.1f;

    public bool IsOpen { get; private set; }

    private Coroutine routine;

    private void Start()
    {
        transform.localRotation = Quaternion.Euler(initialRotation);
    }

    public void Toggle(bool open)
    {
        IsOpen = open;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Rotate(IsOpen ? finalRotation : initialRotation));
    }

    private System.Collections.IEnumerator Rotate(Vector3 target)
    {
        Quaternion start = transform.localRotation;
        Quaternion end = Quaternion.Euler(target);
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(animationTime, 0.0001f);
            transform.localRotation = Quaternion.Lerp(start, end, t);
            yield return null;
        }
        transform.localRotation = end;
        routine = null;
    }
}