using UnityEngine;

public class HoverableObject : MonoBehaviour
{
    public string objectName; // Name of the object to display
    public string objectDescription; // Description of the object to display
    public string Duration => timerNode != null ? timerNode.maxDuration.ToString() : "0"; // Duration from the ITimerNode component
    public string? RedCable;
    public string? BlueCable;
    public string? GreenCable;

    public ITimerNode timerNode; // Reference to the ITimerNode component

    private void Awake()
    {
        // Try to get the ITimerNode component attached to this GameObject
        timerNode = GetComponent<ITimerNode>();
    }


}