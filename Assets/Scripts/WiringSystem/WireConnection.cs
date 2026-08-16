using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
[RequireComponent(typeof(LineRenderer))]
public class WireConnection : MonoBehaviour
{
    private WireNode startNode;
    private WireNode endNode;
    private LineRenderer lineRenderer;

    // =========================================================
    // Curve configuration
    // =========================================================

    private Vector3 planeNormal = Vector3.up;
    public bool isDestructable = false;
    private float wirePlaneOffset = 0.03f;
    private int wireCurveSegments = 18;
    private float wireCurveStrength = 0.12f;
    private float maximumWireCurve = 0.3f;

    // =========================================================
    // Pulse configuration
    // =========================================================

    private GameObject pulsePrefab;
    private float pulseSpeed = 2.5f;
    private float pulseScale = 0.12f;
    private float pulseSurfaceOffset = 0.015f;
    private bool rotatePulseAlongWire = true;

    // =========================================================
    // Timer binding
    // =========================================================

    /*
     * Store the exact event and action references so the
     * connection can be safely removed later.
     */
    private UnityEvent boundSourceEvent;
    private UnityAction boundTargetAction;
    private UnityAction boundConnectionAction;

    public WireNode StartNode => startNode;
    public WireNode EndNode => endNode;
    private WireCollider clickTarget;


    public void Initialize(
        WireNode start,
        WireNode end,
        LineRenderer renderer,
        Vector3 surfaceNormal,
        float planeOffset,
        int curveSegments,
        float curveStrength,
        float maximumCurve,
        GameObject pulseVisualPrefab,
        float pulseMoveSpeed,
        float pulseVisualScale,
        float pulseOffset,
        bool shouldRotatePulse,
        Material permanentWireMats=null,
        bool destructable = true)
    {
        startNode = start;
        endNode = end;
        lineRenderer = renderer;
        
        planeNormal =
            surfaceNormal.sqrMagnitude > 0.0001f
                ? surfaceNormal.normalized
                : Vector3.up;

        wirePlaneOffset = planeOffset;
        wireCurveSegments = Mathf.Max(2, curveSegments);
        wireCurveStrength = curveStrength;
        maximumWireCurve = Mathf.Max(0f, maximumCurve);

        pulsePrefab = pulseVisualPrefab;
        pulseSpeed = Mathf.Max(0.01f, pulseMoveSpeed);
        pulseScale = Mathf.Max(0.01f, pulseVisualScale);
        pulseSurfaceOffset = Mathf.Max(0f, pulseOffset);
        rotatePulseAlongWire = shouldRotatePulse;

        lineRenderer.useWorldSpace = true;
        lineRenderer.numCornerVertices = 4;
        lineRenderer.numCapVertices = 4;
        isDestructable = destructable;

        //Update Material for Indestructable Wire
        if(isDestructable == false){
         MakePermanent(permanentWireMats);
        }

        UpdateWirePosition();
        
        BindTimerConnection();
    }

    // =========================================================
    // Timer connection
    // =========================================================

    private void BindTimerConnection()
    {
        TimerPort sourcePort =
            startNode.GetComponent<TimerPort>();

        TimerPort targetPort =
            endNode.GetComponent<TimerPort>();

        // This may be a normal, non-timer wire.
        if (sourcePort == null && targetPort == null)
            return;

        if (sourcePort == null || targetPort == null)
        {
            Debug.LogWarning(
                "Timer wire requires TimerPort on both endpoints.",
                this
            );

            return;
        }

        if (!sourcePort.CanConnectTo(targetPort))
        {
            Debug.LogWarning(
                $"Invalid timer connection: " +
                $"{sourcePort.PortType} -> {targetPort.PortType}",
                this
            );

            return;
        }

        boundSourceEvent = sourcePort.GetSourceEvent();
        boundTargetAction = targetPort.GetTargetAction();
        Debug.Log($"Binding timer connection: {sourcePort.Timer.name}.{sourcePort.PortType}  -> {targetPort.Timer.name}.{targetPort.PortType} {boundTargetAction.Method.Name}", this);

        if (boundSourceEvent == null ||
            boundTargetAction == null)
        {
            Debug.LogWarning(
                "Timer connection could not find its event or action.",
                this
            );

            ClearBindingReferences();
            return;
        }

        /*
         * When the source activates:
         *
         * 1. Launch the visual pulse.
         * 2. Invoke the target timer action.
         */
        boundConnectionAction = HandleSourceActivated;

        boundSourceEvent.AddListener(
            boundConnectionAction
        );

        Debug.Log(
            $"Registered timer connection: " +
            $"{sourcePort.Timer.name}.{sourcePort.PortType} -> " +
            $"{targetPort.Timer.name}.{targetPort.PortType}",
            this
        );
    }

    private void HandleSourceActivated()
    {
        PlayPulse();
        boundTargetAction?.Invoke();
    }

    // =========================================================
    // Wire positioning
    // =========================================================

    private void LateUpdate()
    {
        if (startNode == null ||
            endNode == null ||
            startNode.WireAnchor == null ||
            endNode.WireAnchor == null)
        {
            Destroy(gameObject);
            return;
        }

        UpdateWirePosition();
    }

    private void UpdateWirePosition()
    {
        if (lineRenderer == null)
            return;

        Vector3 startPosition =
            startNode.WireAnchor.position +
            planeNormal * wirePlaneOffset;

        Vector3 endPosition =
            endNode.WireAnchor.position +
            planeNormal * wirePlaneOffset;

        DrawCurvedWire(
            startPosition,
            endPosition
        );
        clickTarget = GetComponent<WireCollider>();
        clickTarget?.RefreshClickColliders();
    }

    private void DrawCurvedWire(
        Vector3 startPosition,
        Vector3 endPosition)
    {
        int segmentCount =
            Mathf.Max(2, wireCurveSegments);

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = segmentCount;

        Vector3 wireDirection =
            endPosition - startPosition;

        float wireLength =
            wireDirection.magnitude;

        if (wireLength < 0.001f)
        {
            for (int i = 0; i < segmentCount; i++)
            {
                lineRenderer.SetPosition(
                    i,
                    startPosition
                );
            }

            return;
        }

        Vector3 directionOnPlane =
            Vector3.ProjectOnPlane(
                wireDirection,
                planeNormal
            );

        if (directionOnPlane.sqrMagnitude < 0.0001f)
        {
            directionOnPlane = wireDirection;
        }

        Vector3 curveDirection =
            Vector3.Cross(
                planeNormal,
                directionOnPlane.normalized
            ).normalized;

        float curveAmount =
            Mathf.Clamp(
                wireLength * wireCurveStrength,
                -maximumWireCurve,
                maximumWireCurve
            );

        Vector3 midpoint =
            Vector3.Lerp(
                startPosition,
                endPosition,
                0.5f
            );

        Vector3 controlPoint =
            midpoint +
            curveDirection * curveAmount;

        for (int i = 0; i < segmentCount; i++)
        {
            float t =
                i / (float)(segmentCount - 1);

            float inverseT =
                1f - t;

            Vector3 position =
                inverseT * inverseT * startPosition +
                2f * inverseT * t * controlPoint +
                t * t * endPosition;

            lineRenderer.SetPosition(
                i,
                position
            );
        }
    }

    public void MakePermanent(Material permanentMaterial){
        if(permanentMaterial)
       { List<Material> materials = new List<Material>();
        lineRenderer.GetMaterials(materials);
        materials.Add(permanentMaterial);
        lineRenderer.SetMaterials(materials);}
    }
    // =========================================================
    // Pulse
    // =========================================================

    public void PlayPulse()
    {
        if (pulsePrefab == null ||
            lineRenderer == null ||
            lineRenderer.positionCount < 2)
        {
            return;
        }

        GameObject pulse =
            Instantiate(
                pulsePrefab,
                transform
            );

        pulse.transform.localScale =
            Vector3.one * pulseScale;

        StartCoroutine(
            MovePulseAlongWire(pulse)
        );
    }


        public void PlayRejectCancel()
    {
        //TODO: add animation to show cant disconnect
        if (pulsePrefab == null ||
            lineRenderer == null ||
            lineRenderer.positionCount < 2)
        {
            return;
        }

        GameObject pulse =
            Instantiate(
                pulsePrefab,
                transform
            );

        pulse.transform.localScale =
            Vector3.one * pulseScale;

        StartCoroutine(
            MovePulseAlongWire(pulse)
        );
    }

    private IEnumerator MovePulseAlongWire(
        GameObject pulse)
    {
        float normalizedDistance = 0f;

        while (pulse != null &&
               lineRenderer != null &&
               normalizedDistance < 1f)
        {
            if (!TryGetWirePoint(
                    normalizedDistance,
                    out Vector3 position,
                    out Vector3 direction,
                    out float wireLength))
            {
                break;
            }

            pulse.transform.position =
                position +
                planeNormal * pulseSurfaceOffset;

            if (rotatePulseAlongWire &&
                direction.sqrMagnitude > 0.0001f)
            {
                /*
                 * Assumes the arrow prefab points along
                 * its local positive Z axis.
                 */
                pulse.transform.rotation =
                    Quaternion.LookRotation(
                        direction,
                        planeNormal
                    );
            }

            normalizedDistance +=
                pulseSpeed *
                Time.deltaTime /
                Mathf.Max(wireLength, 0.001f);

            yield return null;
        }

        if (pulse != null)
        {
            /*
             * Place it exactly at the target for the final frame.
             */
            if (TryGetWirePoint(
                    1f,
                    out Vector3 finalPosition,
                    out Vector3 finalDirection,
                    out _))
            {
                pulse.transform.position =
                    finalPosition +
                    planeNormal * pulseSurfaceOffset;

                if (rotatePulseAlongWire &&
                    finalDirection.sqrMagnitude > 0.0001f)
                {
                    pulse.transform.rotation =
                        Quaternion.LookRotation(
                            finalDirection,
                            planeNormal
                        );
                }
            }

            Destroy(pulse);
        }
    }

    private bool TryGetWirePoint(
        float normalizedDistance,
        out Vector3 position,
        out Vector3 direction,
        out float totalLength)
    {
        position = default;
        direction = Vector3.forward;
        totalLength = 0f;

        if (lineRenderer == null ||
            lineRenderer.positionCount < 2)
        {
            return false;
        }

        int pointCount =
            lineRenderer.positionCount;

        /*
         * First pass: find the total curved-wire length.
         */
        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 pointA =
                lineRenderer.GetPosition(i);

            Vector3 pointB =
                lineRenderer.GetPosition(i + 1);

            totalLength +=
                Vector3.Distance(pointA, pointB);
        }

        if (totalLength < 0.0001f)
        {
            position =
                lineRenderer.GetPosition(0);

            return true;
        }

        float targetDistance =
            Mathf.Clamp01(normalizedDistance) *
            totalLength;

        float travelledDistance = 0f;

        /*
         * Second pass: find the segment containing the
         * requested distance.
         */
        for (int i = 0; i < pointCount - 1; i++)
        {
            Vector3 pointA =
                lineRenderer.GetPosition(i);

            Vector3 pointB =
                lineRenderer.GetPosition(i + 1);

            float segmentLength =
                Vector3.Distance(pointA, pointB);

            if (travelledDistance + segmentLength >=
                targetDistance)
            {
                float distanceInsideSegment =
                    targetDistance -
                    travelledDistance;

                float segmentT =
                    segmentLength > 0.0001f
                        ? distanceInsideSegment /
                          segmentLength
                        : 0f;

                position =
                    Vector3.Lerp(
                        pointA,
                        pointB,
                        segmentT
                    );

                direction =
                    (pointB - pointA).normalized;

                return true;
            }

            travelledDistance += segmentLength;
        }

        Vector3 previousPoint =
            lineRenderer.GetPosition(pointCount - 2);

        Vector3 finalPoint =
            lineRenderer.GetPosition(pointCount - 1);

        position = finalPoint;
        direction =
            (finalPoint - previousPoint).normalized;

        return true;
    }

    // =========================================================
    // Cleanup
    // =========================================================

    private void OnDestroy()
    {
        UnbindTimerConnection();
    }

    private void UnbindTimerConnection()
    {
        if (boundSourceEvent != null &&
            boundConnectionAction != null)
        {
            startNode.RemoveEndReference(endNode);
            boundSourceEvent.RemoveListener(
                boundConnectionAction
            );
        }
        ClearBindingReferences();
    }
    public void DestroyConnection()
    {
        UnbindTimerConnection();
        Destroy(gameObject);
    }

    private void ClearBindingReferences()
    {
        boundSourceEvent = null;
        boundTargetAction = null;
        boundConnectionAction = null;
    }
}