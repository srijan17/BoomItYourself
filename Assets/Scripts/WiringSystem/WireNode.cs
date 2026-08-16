using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
[Serializable]
public class WireNodeEvent : UnityEvent<WireNode>
{
}

[RequireComponent(typeof(Collider))]
public class WireNode : MonoBehaviour
{
    [Header("Connection Settings")]
    [SerializeField] private bool canStartWire = true;
    [SerializeField] private bool canReceiveWire = true;

    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color originalColor;

    [Tooltip("Where the wire visually attaches. Uses this transform if empty.")]
    [SerializeField] private Transform wireAnchor;

    [Header("Events")]
    [Tooltip("Called on this node when it connects to another node.")]
    public WireNodeEvent onWireSent;

    [Tooltip("Called when another node connects to this node.")]
    public WireNodeEvent onWireReceived;

    public Transform WireAnchor => wireAnchor != null ? wireAnchor : transform;

    public bool CanStartWire => canStartWire;
    public bool CanReceiveWire => canReceiveWire;

    public List<WireNode> ConnectedNodes { get; private set; } = new List<WireNode>();

    /// <summary>
    /// Override this in special node types to add connection rules.
    /// </summary>
    // public virtual bool CanConnectTo(WireNode target)
    // {
    //     if (target == null)
    //         return false;

    //     if (target == this)
    //         return false;

    //     if (!canStartWire)
    //         return false;

    //     if (!target.canReceiveWire)
    //         return false;

    //     return true;
    // }

    public void Awake()
    {
        originalColor = GetComponent<Renderer>().material.color;
    }
    /// <summary>
    /// Called when this node successfully connects to another node.
    /// </summary>
    public virtual bool ConnectTo(WireNode target)
    {
        if(ConnectedNodes.Contains(target))
        {
            Debug.LogWarning($"{name} is already connected to {target.name}. Connection skipped.");
            return false;
        }
        Debug.Log($"{name} connected to {target.name}");

        onWireSent?.Invoke(target);
        ConnectedNodes.Add(target);
        target.ReceiveConnection(this);
        return true;
    }

    public void RemoveEndReference(WireNode target)
    {
        if (ConnectedNodes.Contains(target))
        {
            ConnectedNodes.Remove(target);
        }
    }

    protected virtual void ReceiveConnection(WireNode source)
    {
        onWireReceived?.Invoke(source);
    }

public virtual bool CanConnectTo(WireNode target)
{
    
    if (target == null)
        return false;

    if (target == this)
        return false;

    if (!canStartWire)
        return false;

    if (!target.canReceiveWire)
        return false;

    TimerPort sourceTimerPort = GetComponent<TimerPort>();
    TimerPort targetTimerPort = target.GetComponent<TimerPort>();

    /*
     * If either node is a timer port, both nodes must be timer ports
     * and their port combination must be valid.
     */
    if (sourceTimerPort != null || targetTimerPort != null)
    {
        if (sourceTimerPort == null || targetTimerPort == null)
            return false;

        return sourceTimerPort.CanConnectTo(targetTimerPort);
    }

    return true;
}


public void HighlightNode(bool highlight)
{
    // Implement highlighting logic here, e.g., change material or color
    Renderer renderer = GetComponent<Renderer>();
    if (renderer != null)
    {
        if (highlight)
        {
            // Change to highlight material or color
            renderer.material.color = highlightColor; // Example highlight color
        }
        else
        {
            // Revert to original material or color
            renderer.material.color = originalColor; // Example original color
        }
    }

}
}
