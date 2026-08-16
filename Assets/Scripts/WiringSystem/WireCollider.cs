using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class WireCollider : MonoBehaviour
{
    [Header("Click Area")]
    [SerializeField, Min(0.01f)]
    private float clickRadius = 0.12f;

    [SerializeField]
    private UnityEvent onWireClicked;

    private LineRenderer lineRenderer;
    private readonly List<CapsuleCollider> segmentColliders = new();

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        RefreshClickColliders();
    }

    /// <summary>
    /// Call this whenever the LineRenderer positions change.
    /// </summary>
    public void RefreshClickColliders()
    {
        int requiredColliderCount =
            Mathf.Max(0, lineRenderer.positionCount - 1);

        EnsureColliderCount(requiredColliderCount);

        for (int i = 0; i < segmentColliders.Count; i++)
        {
            bool shouldBeActive = i < requiredColliderCount;
            segmentColliders[i].gameObject.SetActive(shouldBeActive);

            if (!shouldBeActive)
                continue;

            Vector3 start = GetWorldPosition(i);
            Vector3 end = GetWorldPosition(i + 1);

            UpdateCollider(segmentColliders[i], start, end);
        }
    }

    private Vector3 GetWorldPosition(int index)
    {
        Vector3 position = lineRenderer.GetPosition(index);

        return lineRenderer.useWorldSpace
            ? position
            : transform.TransformPoint(position);
    }

    private void EnsureColliderCount(int requiredCount)
    {
        while (segmentColliders.Count < requiredCount)
        {
            GameObject hitbox = new GameObject(
                $"Wire Click Collider {segmentColliders.Count}");

            hitbox.layer = gameObject.layer;

            // Keep its world transform independent from the wire's scaling.
            hitbox.transform.SetParent(transform, true);

            CapsuleCollider collider =
                hitbox.AddComponent<CapsuleCollider>();

            collider.direction = 2; // Local Z axis.
            collider.isTrigger = true;

            WireClickProxy proxy =
                hitbox.AddComponent<WireClickProxy>();

            proxy.Initialize(this);

            segmentColliders.Add(collider);
        }
    }

    private void UpdateCollider(
        CapsuleCollider collider,
        Vector3 start,
        Vector3 end)
    {
        Vector3 direction = end - start;
        float length = direction.magnitude;

        if (length <= Mathf.Epsilon)
        {
            collider.gameObject.SetActive(false);
            return;
        }

        Transform colliderTransform = collider.transform;

        colliderTransform.position = (start + end) * 0.5f;
        colliderTransform.rotation =
            Quaternion.FromToRotation(Vector3.forward, direction.normalized);

        colliderTransform.localScale = Vector3.one;

        collider.center = Vector3.zero;
        collider.radius = clickRadius;

        // Capsule height includes both rounded ends.
        collider.height = length + clickRadius * 2f;
    }

    public void HandleWireClicked()
    {
        Debug.Log($"Clicked wire: {name}");

        onWireClicked?.Invoke();
    }
}