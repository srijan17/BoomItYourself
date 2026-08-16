using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.InputSystem;
public class WireManager : MonoBehaviour
{

    private PlayerInputActions inputActions;

    private Vector2 PointerScreenPosition =>
        inputActions.Workbench.Point.ReadValue<Vector2>();

    private bool PrimaryPressedThisFrame =>
        inputActions.Workbench.Primary.WasPressedThisFrame();

    private bool SecondaryPressedThisFrame =>
        inputActions.Workbench.Secondary.WasPressedThisFrame();


    [Header("References")]
    [SerializeField]
    private Camera wiringCamera;

    [Tooltip("Prefab containing a LineRenderer.")]
    [SerializeField]
    private LineRenderer wirePrefab;

    [Tooltip(
        "The shared board/table plane. Its local UP direction " +
        "must point outward from the board surface."
    )]
    [SerializeField]
    private Transform wiringPlane;

    [Header("Raycast Layers")]
    [SerializeField]
    private LayerMask wireNodeLayerMask;

    [SerializeField]
    private LayerMask movableObjectLayerMask;


    [SerializeField]
    private LayerMask wireConnectionLayerMask;


    [Header("Wire")]
    [Tooltip("Prevents the wire from visually clipping into the board.")]
    [SerializeField]
    private float wirePlaneOffset = 0.03f;

    [Header("Wire Pulse")]
    [Tooltip("Small yellow orb, diamond or arrow prefab.")]
    [SerializeField]
    private GameObject wirePulsePrefab;

    [SerializeField, Min(0.01f)]
    private float wirePulseSpeed = 2.5f;

    [SerializeField, Min(0.01f)]
    private float wirePulseScale = 0.12f;

    [Tooltip("Lifts the pulse slightly above the wire.")]
    [SerializeField, Min(0f)]
    private float wirePulseSurfaceOffset = 0.015f;

    [Tooltip(
        "Enable for an arrow prefab whose forward direction is local Z."
    )]
    [SerializeField]
    private bool rotatePulseAlongWire = true;


    [Header("Wire Curve")]

    [SerializeField, Range(4, 40)]
    private int wireCurveSegments = 18;

    [Tooltip("Curve strength relative to the wire length.")]
    [SerializeField, Range(-0.5f, 0.5f)]
    private float wireCurveStrength = 0.15f;

    [Tooltip("Maximum amount the wire can bend.")]
    [SerializeField, Min(0f)]
    private float maximumWireCurve = 0.35f;


    [Header("Interaction Bounds")]
    [SerializeField]
    private WireInteractionBoundary interactionBounds;

    // [Header("Node Info Panel")]
    // [SerializeField]
    // private NodeInfoPanelRederer nodeInfoPanel;

    public Texture2D grabCursorTexture;
    public Texture2D normalCursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public bool isHovering = false;

    // public DungeonSimulation DungeonSimulation;

    public AudioSource audioSource;


    /*
     * This is the mathematical plane used for projecting
     * the mouse ray onto the board.
     */
    private Plane interactionPlane;

    // =========================================================
    // Wire state
    // =========================================================

    private WireNode startNode;
    private LineRenderer previewWire;

    // =========================================================
    // Movement state
    // =========================================================

    private MovableWireObject movingObject;
    private Transform movingRoot;

    private Vector3 moveGrabOffset;
    private Vector3 moveStartPosition;

    private bool IsDrawingWire =>
        startNode != null &&
        previewWire != null;

    private bool IsMovingObject =>
        movingObject != null &&
        movingRoot != null;

    private HoverableObject lastHightlightedObject;

    //TODO: Move to wire connection probably
    [SerializeField] private Material permanentWireMats;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        if (wiringCamera == null)
        {
            wiringCamera = Camera.main;
        }

        audioSource = GetComponent<AudioSource>();
    
    }

    private void OnEnable()
    {
        inputActions.Workbench.Enable();
    }

    private void OnDisable()
    {
        inputActions?.Workbench.Disable();

        if (IsDrawingWire)
        {
            CancelWire();
        }

        if (IsMovingObject)
        {
            CancelMovingObject();
        }
    }

    private void OnDestroy()
    {
        inputActions?.Dispose();
    }

    private void Update()
    {
        // if(!DungeonSimulation.isSimulationRunning){
        // isHovering=false;
        bool pointerOverUI =
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject();

        if (IsDrawingWire)
        {
            HandleWireDrawing(pointerOverUI);
            return;
        }

        if (IsMovingObject)
        {
            HandleObjectMovement(pointerOverUI);
            return;
        }

        if (pointerOverUI)
            return;

        if (PrimaryPressedThisFrame)
        {
            HandleInitialLeftClick();
        }
        else if (SecondaryPressedThisFrame)
        {
            HandleInitialRightClick();
        }
        else{
            HandleHover();
        }

        // if(isHovering){
        //     Cursor.SetCursor(grabCursorTexture, hotSpot, cursorMode);
           
        // }
        // else{
        //     Cursor.SetCursor(normalCursorTexture, hotSpot, cursorMode);
        // }
        // }
        
    }

    private void HandleInitialRightClick()
    {
        WireConnection clickedWire = GetWireUnderMouse();

        if (clickedWire != null )
        {
            if(clickedWire.isDestructable)
            {
                clickedWire.DestroyConnection();
                }
            else{
                clickedWire.PlayRejectCancel();
                //Pulse that its not destructable
            }
            return;
        }

    }
    private void HandleInitialLeftClick()
    {
        /*
         * Nodes are checked before movable objects.
         *
         * Clicking a connection socket therefore starts a wire
         * instead of moving the timer object behind it.
         */
        WireNode clickedNode = GetNodeUnderMouse();

        if (clickedNode != null)
        {
            if (clickedNode.CanStartWire)
            {
                StartWire(clickedNode);
            }

            return;
        }

        MovableWireObject clickedObject =
            GetMovableObjectUnderMouse();

        if (clickedObject != null && clickedObject.CanMove)
        {
            StartMovingObject(clickedObject);
        }
      
        
    }



       private void HandleHover()
    {
        /*
         * Nodes are checked before movable objects.
         *
         * Clicking a connection socket therefore starts a wire
         * instead of moving the timer object behind it.
         */

        WireNode hoverNode = GetNodeUnderMouse();
        HoverableObject hoverObject =
            GetHoverableObjectUnderMouse();
        // if(hoverNode != null || hoverObject!=null )
        // {
        //     MovableWireObject movableObject = hoverNode.GetComponent<MovableWireObject>();

        // }

       
        
        if(lastHightlightedObject!=null && hoverObject==null){
            UnHighlightHoverableObject(lastHightlightedObject);
        }
        else if(lastHightlightedObject!=null && hoverObject!=null  &&  
        hoverObject.gameObject.GetEntityId()!=lastHightlightedObject.gameObject.GetEntityId()){
            UnHighlightHoverableObject(lastHightlightedObject);
        }

        if (hoverObject != null )
        {
            HighlightHoverableObject(hoverObject);
            lastHightlightedObject = hoverObject;
            
        }
      
        
    }

    // =========================================================
    // Wire interaction
    // =========================================================

    private void HandleWireDrawing(bool pointerOverUI)
    {
        HighlightValidWireTargets();
        UpdatePreviewWire();

        if (SecondaryPressedThisFrame)
        {
            ClearWireHighlights();
            CancelWire();
            return;
        }

        if (!pointerOverUI && PrimaryPressedThisFrame)
        {
            TryCompleteWire();
        }
    }

    private void StartWire(WireNode node)
    {
        if (node == null || node.WireAnchor == null)
            return;

        if (interactionBounds != null &&
            !interactionBounds.ContainsWorldPosition(
                node.WireAnchor.position))
        {
            return;
        }

        startNode = node;

        CreateInteractionPlane(node.WireAnchor.position);

        previewWire = Instantiate(wirePrefab);
        previewWire.useWorldSpace = true;

        // Makes thick LineRenderers look smoother.
        previewWire.numCornerVertices = 4;
        previewWire.numCapVertices = 4;

        Vector3 startPosition =
            node.WireAnchor.position +
            interactionPlane.normal * wirePlaneOffset;

        DrawCurvedWire(
            previewWire,
            startPosition,
            startPosition
        );
    }

    private void HighlightValidWireTargets()
    {
        WireNode[] allNodes =
            FindObjectsOfType<WireNode>();

        foreach (WireNode node in allNodes)
        {
            if (node == null || node.WireAnchor == null)
                continue;

            bool isValidTarget =
                startNode.CanConnectTo(node);

            if (interactionBounds != null &&
                !interactionBounds.ContainsWorldPosition(
                    node.WireAnchor.position))
            {
                isValidTarget = false;
            }

            // Highlight the node based on its validity.
            node.HighlightNode(isValidTarget);
        }
    }

    private void ClearWireHighlights()
    {
        WireNode[] allNodes =
            FindObjectsOfType<WireNode>();

        foreach (WireNode node in allNodes)
        {
            if (node == null)
                continue;

            node.HighlightNode(false);
        }
    }
    private void UpdatePreviewWire()
    {
        if (previewWire == null ||
            startNode == null ||
            startNode.WireAnchor == null)
        {
            return;
        }

        if (!TryGetMouseWorldPosition(out Vector3 mousePosition))
            return;

        if (interactionBounds != null)
        {
            mousePosition =
                interactionBounds.ClampWorldPosition(
                    mousePosition,
                    0f
                );
        }

        Vector3 planeNormal =
            interactionPlane.normal;

        Vector3 startPosition =
            startNode.WireAnchor.position +
            planeNormal * wirePlaneOffset;

        Vector3 endPosition =
            mousePosition +
            planeNormal * wirePlaneOffset;

        DrawCurvedWire(
            previewWire,
            startPosition,
            endPosition
        );
    }

    private void TryCompleteWire()
    {
        WireNode targetNode = GetNodeUnderMouse();

        // Clicking empty space leaves the preview active.
        if (targetNode == null)
            return;

        if (targetNode.WireAnchor == null)
            return;

        /*
         * Do not allow connections to nodes outside the
         * interaction bounds.
         */
        if (interactionBounds != null &&
            !interactionBounds.ContainsWorldPosition(
                targetNode.WireAnchor.position))
        {
            return;
        }

        if (!startNode.CanConnectTo(targetNode))
            return;
        ClearWireHighlights();
        CompleteWire(targetNode);
    }

    private void CompleteWire(WireNode targetNode)
    {
        audioSource.PlayOneShot(audioSource.clip);
        Vector3 planeNormal =
            interactionPlane.normal;

        Vector3 startPosition =
            startNode.WireAnchor.position +
            planeNormal * wirePlaneOffset;

        Vector3 endPosition =
            targetNode.WireAnchor.position +
            planeNormal * wirePlaneOffset;

        DrawCurvedWire(
            previewWire,
            startPosition,
            endPosition
        );

        WireConnection connection =
            previewWire.GetComponent<WireConnection>();
        bool wireConnected = startNode.ConnectTo(targetNode);
        if(!wireConnected){
            Debug.LogWarning($"Connection from {startNode.name} to {targetNode.name} was not established. Connection skipped.");
            Destroy(previewWire.gameObject);
            previewWire = null;
            startNode = null;
            return;
        }
        if (connection == null)
        {
            connection =
                previewWire.gameObject
                    .AddComponent<WireConnection>();
        }
        
        connection.Initialize(
            startNode,
            targetNode,
            previewWire,
            interactionPlane.normal,
            wirePlaneOffset,
            wireCurveSegments,
            wireCurveStrength,
            maximumWireCurve,
            wirePulsePrefab,
            wirePulseSpeed,
            wirePulseScale,
            wirePulseSurfaceOffset,
            rotatePulseAlongWire
        );


        previewWire = null;
        startNode = null;
    }


    public  void MakeCompleteWire(ITimerNode source , ITimerNode destination)
    {
        TimerPort[] startTimerPorts = source.GetComponentsInChildren<TimerPort>();
        WireNode startWireNode = startTimerPorts.ToList().Find(x => x.PortType == TimerPortType.Complete).GetComponent<WireNode>();
        Debug.Log(startWireNode.gameObject.name);

        TimerPort[] endTimerPorts = destination.GetComponentsInChildren<TimerPort>();
        WireNode endWireNode = endTimerPorts.ToList().
        Find(x => x.PortType == TimerPortType.Start || x.PortType == TimerPortType.Pause)
        .GetComponent<WireNode>();

        // Debug.Log(endWireNode.gameObject.name);
        // // audioSource.PlayOneShot(audioSource.clip);
        Vector3 planeNormal =
            interactionPlane.normal;

        Vector3 startPosition =
            startWireNode.WireAnchor.position +
            planeNormal * wirePlaneOffset;

        Vector3 endPosition =
            endWireNode.WireAnchor.position +
            planeNormal * wirePlaneOffset;

       

        previewWire = Instantiate(wirePrefab);
        previewWire.useWorldSpace = true;

        // Makes thick LineRenderers look smoother.
        previewWire.numCornerVertices = 4;
        previewWire.numCapVertices = 4;

        DrawCurvedWire(
            previewWire,
            startPosition,
            endPosition
        );
        

        WireConnection connection =
            previewWire.GetComponent<WireConnection>();
        
        if (connection == null)
        {
            connection =
                previewWire.gameObject
                    .AddComponent<WireConnection>();
        }

        connection.Initialize(
            startWireNode,
            endWireNode,
            previewWire,
            interactionPlane.normal,
            wirePlaneOffset,
            wireCurveSegments,
            wireCurveStrength,
            maximumWireCurve,
            wirePulsePrefab,
            wirePulseSpeed,
            wirePulseScale,
            wirePulseSurfaceOffset,
            rotatePulseAlongWire,
            permanentWireMats,
            false
        );

        startWireNode.ConnectTo(endWireNode);
        previewWire = null;
    }

    private void DrawCurvedWire(
        LineRenderer lineRenderer,
        Vector3 startPosition,
        Vector3 endPosition)
    {
        if (lineRenderer == null)
            return;

        int segmentCount =
            Mathf.Max(2, wireCurveSegments);

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = segmentCount;

        Vector3 wireDirection =
            endPosition - startPosition;

        float wireLength =
            wireDirection.magnitude;

        /*
        * While the wire has no length, place every point at
        * the start to prevent invalid normalized vectors.
        */
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

        Vector3 planeNormal =
            interactionPlane.normal.normalized;

        /*
        * Keep the direction flat against the interaction plane.
        */
        Vector3 directionOnPlane =
            Vector3.ProjectOnPlane(
                wireDirection,
                planeNormal
            );

        if (directionOnPlane.sqrMagnitude < 0.0001f)
        {
            directionOnPlane = wireDirection;
        }

        /*
        * A perpendicular direction lying along the board.
        *
        * This produces a sideways curve rather than lifting the
        * wire away from the table.
        */
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

            /*
            * Quadratic Bézier:
            *
            * start -> control point -> end
            */
            Vector3 position =
                inverseT * inverseT * startPosition +
                2f * inverseT * t * controlPoint +
                t * t * endPosition;

            lineRenderer.SetPosition(i, position);
        }
    }
    private void CancelWire()
    {
        audioSource.PlayOneShot(audioSource.clip);
        if (previewWire != null)
        {
            Destroy(previewWire.gameObject);
        }

        previewWire = null;
        startNode = null;
    }

    // =========================================================
    // Object movement
    // =========================================================

    private void HandleObjectMovement(bool pointerOverUI)
    {
        UpdateMovingObject();

        if (SecondaryPressedThisFrame)
        {
            CancelMovingObject();
            return;
        }

        if (!pointerOverUI && PrimaryPressedThisFrame)
        {
            PlaceMovingObject();
        }
    }

    private void StartMovingObject(
        MovableWireObject movableObject)
    {
        if (movableObject == null)
            return;

        movingObject = movableObject;
        movingRoot = movableObject.MoveRoot;

        if (movingRoot == null)
        {
            ClearMovementState();
            return;
        }

        moveStartPosition = movingRoot.position;

        CreateInteractionPlane(movingRoot.position);

        /*
         * Preserve the exact point where the player grabbed
         * the object so its centre does not snap to the cursor.
         */
        if (TryGetMouseWorldPosition(out Vector3 mousePosition))
        {
            moveGrabOffset =
                movingRoot.position - mousePosition;
        }
        else
        {
            moveGrabOffset = Vector3.zero;
        }

        movingObject.NotifyPickedUp();
    }

    private void HighlightHoverableObject(HoverableObject hoverableObject)
    {
        if (hoverableObject == null)
            return;
        
        // nodeInfoPanel.DisplayNodeInfo(hoverableObject);

        MovableWireObject movableObject = hoverableObject.GetComponent<MovableWireObject>();
        if(movableObject != null)
        {
            HighlightMovableObject(movableObject);
        }
    }

    private void HighlightMovableObject(
        MovableWireObject movableObject)
    {


        if(movableObject != null && movableObject.CanMove)
           {
            movableObject.Highlight();}

        
    }

    private void UnHighlightHoverableObject(HoverableObject hoverableObject)
    {
        
        if (hoverableObject == null)
            return;

        // if(nodeInfoPanel != null)
        // {
        //     nodeInfoPanel.HidePanel();
        // }
        MovableWireObject movableObject = hoverableObject.gameObject.GetComponent<MovableWireObject>();
        if(movableObject != null)
        {
            movableObject.Unhighlight();
        }
    }
    private void UnHighlightMovableObject(
        MovableWireObject movableObject)
    {
        if (movableObject == null)
            return;

        movingObject = movableObject;
        movingObject.Unhighlight();
        

    }

    private void UpdateMovingObject()
    {
        if (movingRoot == null)
        {
            ClearMovementState();
            return;
        }

        if (!TryGetMouseWorldPosition(out Vector3 mousePosition))
            return;

        Vector3 targetPosition =
            mousePosition + moveGrabOffset;

        /*
         * This is the original bounding-box movement logic.
         */
        if (interactionBounds != null)
        {
            targetPosition =
                interactionBounds.ClampWorldPosition(
                    targetPosition,
                    0f
                );
        }

        movingRoot.position = targetPosition;
    }

    private void PlaceMovingObject()
    {
        audioSource.PlayOneShot(audioSource.clip);
        if (movingObject != null)
        {
            movingObject.NotifyPlaced();
        }

        ClearMovementState();
    }

    private void CancelMovingObject()
    {
        if (movingRoot != null)
        {
            movingRoot.position = moveStartPosition;
        }

        if (movingObject != null)
        {
            movingObject.NotifyMoveCancelled();
        }

        ClearMovementState();
    }

    private void ClearMovementState()
    {
        movingObject = null;
        movingRoot = null;
        moveGrabOffset = Vector3.zero;
    }

    // =========================================================
    // Plane and raycast helpers
    // =========================================================

    private void CreateInteractionPlane(
        Vector3 interactionPosition)
    {
        if (wiringPlane != null)
        {
            /*
             * A standard Unity Plane or horizontal table uses
             * local X and Z as its surface axes.
             *
             * Local Y/up is therefore its surface normal.
             */
            interactionPlane = new Plane(
                wiringPlane.up,
                wiringPlane.position
            );
        }
        else
        {
            /*
             * Fallback:
             * Make a plane parallel to the camera screen,
             * passing through the clicked object.
             */
            interactionPlane = new Plane(
                -wiringCamera.transform.forward,
                interactionPosition
            );
        }
    }

    private bool TryGetMouseWorldPosition(
        out Vector3 worldPosition)
    {
        if (wiringCamera == null)
        {
            worldPosition = default;
            return false;
        }

        Ray ray =
            wiringCamera.ScreenPointToRay(
                PointerScreenPosition
            );

        if (interactionPlane.Raycast(
                ray,
                out float distance))
        {
            worldPosition = ray.GetPoint(distance);
            return true;
        }

        worldPosition = default;
        return false;
    }

    private WireNode GetNodeUnderMouse()
    {
        if (wiringCamera == null)
            return null;

        Ray ray =
            wiringCamera.ScreenPointToRay(
                PointerScreenPosition
            );

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                Mathf.Infinity,
                wireNodeLayerMask))
        {
            return null;
        }

        return hit.collider
            .GetComponentInParent<WireNode>();
    }

    private MovableWireObject GetMovableObjectUnderMouse()
    {
        if (wiringCamera == null)
            return null;

        Ray ray =
            wiringCamera.ScreenPointToRay(
                PointerScreenPosition
            );

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                Mathf.Infinity,
                movableObjectLayerMask))
        {
            return null;
        }

        return hit.collider
            .GetComponentInParent<MovableWireObject>();
    }

    private HoverableObject GetHoverableObjectUnderMouse()
    {
        if (wiringCamera == null)
            return null;

        Ray ray =
            wiringCamera.ScreenPointToRay(
                PointerScreenPosition
            );

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                Mathf.Infinity,
                movableObjectLayerMask))
        {
            return null;
        }

        return hit.collider
            .GetComponentInParent<HoverableObject>();
    }



    private WireConnection GetWireUnderMouse()
    {
        if (wiringCamera == null)
            return null;

        Ray ray =
            wiringCamera.ScreenPointToRay(
                PointerScreenPosition
            );

        if (!Physics.Raycast(
                ray,
                out RaycastHit hit,
                Mathf.Infinity,
                wireConnectionLayerMask))
            {
                return null;
            }

        return hit.collider
            .GetComponentInParent<WireConnection>();
    }
  
}