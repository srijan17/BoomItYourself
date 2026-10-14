using UnityEngine;

public abstract class ITarget : MonoBehaviour
{
    public  string targetName;
    public abstract void NewEvent(SimulationEvent simEvent);
    public abstract void ResetTarget();
}