using UnityEngine;

public abstract class ITarget : MonoBehaviour
{
    public abstract string targetName { get; set; }
    public abstract void NewEvent(SimulationEvent simEvent);
    public abstract void ResetTarget();
}