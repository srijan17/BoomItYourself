using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WireInteractionBoundary : MonoBehaviour
{
    [Header("Bounds Collider")]
    [SerializeField]
    private BoxCollider boundsCollider;

    [Tooltip(
        "Small tolerance used when checking whether a point " +
        "is inside the interaction area."
    )]
    [SerializeField]
    private float containsTolerance = 0.001f;

    private void Awake()
    {
        EnsureCollider();
    }

    private void Reset()
    {
        EnsureCollider();

        if (boundsCollider != null)
        {
            boundsCollider.isTrigger = true;
        }
    }

    private void EnsureCollider()
    {
        if (boundsCollider == null)
        {
            boundsCollider = GetComponent<BoxCollider>();
        }
    }

    /// <summary>
    /// Clamps a world position inside the bounds using
    /// local X and local Z as the board surface axes.
    ///
    /// Local Y is preserved so the object does not change height.
    /// </summary>
    public Vector3 ClampWorldPosition(
        Vector3 worldPosition,
        float worldPadding = 0f)
    {
        EnsureCollider();

        if (boundsCollider == null)
            return worldPosition;

        Transform boundsTransform =
            boundsCollider.transform;

        Vector3 localPosition =
            boundsTransform.InverseTransformPoint(worldPosition);

        Vector3 localCenter =
            boundsCollider.center;

        Vector3 localHalfSize =
            boundsCollider.size * 0.5f;

        /*
         * BoundsPadding is supplied in world units.
         * Convert it separately for the local X and Z axes.
         */
        Vector3 scale = boundsTransform.lossyScale;

        float localPaddingX =
            worldPadding /
            Mathf.Max(Mathf.Abs(scale.x), 0.0001f);

        float localPaddingZ =
            worldPadding /
            Mathf.Max(Mathf.Abs(scale.z), 0.0001f);

        float minX =
            localCenter.x -
            localHalfSize.x +
            localPaddingX;

        float maxX =
            localCenter.x +
            localHalfSize.x -
            localPaddingX;

        float minZ =
            localCenter.z -
            localHalfSize.z +
            localPaddingZ;

        float maxZ =
            localCenter.z +
            localHalfSize.z -
            localPaddingZ;

        /*
         * Prevent invalid Clamp ranges if padding is larger
         * than half the interaction area.
         */
        if (minX > maxX)
        {
            minX = localCenter.x;
            maxX = localCenter.x;
        }

        if (minZ > maxZ)
        {
            minZ = localCenter.z;
            maxZ = localCenter.z;
        }

        localPosition.x =
            Mathf.Clamp(localPosition.x, minX, maxX);

        localPosition.z =
            Mathf.Clamp(localPosition.z, minZ, maxZ);

        /*
         * Do not clamp localPosition.y.
         * Y is the height above the table.
         */
        return boundsTransform.TransformPoint(localPosition);
    }

    /// <summary>
    /// Checks whether a world position is inside the board's
    /// local XZ interaction area.
    ///
    /// Height is intentionally ignored.
    /// </summary>
    public bool ContainsWorldPosition(Vector3 worldPosition)
    {
        EnsureCollider();

        if (boundsCollider == null)
            return true;

        Transform boundsTransform =
            boundsCollider.transform;

        Vector3 localPosition =
            boundsTransform.InverseTransformPoint(worldPosition);

        Vector3 localCenter =
            boundsCollider.center;

        Vector3 localHalfSize =
            boundsCollider.size * 0.5f;

        bool insideX =
            localPosition.x >=
                localCenter.x -
                localHalfSize.x -
                containsTolerance &&
            localPosition.x <=
                localCenter.x +
                localHalfSize.x +
                containsTolerance;

        bool insideZ =
            localPosition.z >=
                localCenter.z -
                localHalfSize.z -
                containsTolerance &&
            localPosition.z <=
                localCenter.z +
                localHalfSize.z +
                containsTolerance;

        return insideX && insideZ;
    }

    private void OnDrawGizmosSelected()
    {
        EnsureCollider();

        if (boundsCollider == null)
            return;

        Matrix4x4 previousMatrix = Gizmos.matrix;

        Gizmos.matrix =
            boundsCollider.transform.localToWorldMatrix;

        Gizmos.DrawWireCube(
            boundsCollider.center,
            boundsCollider.size
        );

        Gizmos.matrix = previousMatrix;
    }
}