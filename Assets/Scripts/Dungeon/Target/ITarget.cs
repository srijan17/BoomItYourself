using UnityEngine;

public enum TargetType
{
    Door,
    PressurePlate,
    Portal,
    Bomb,
    Hero
}
public abstract class ITarget : MonoBehaviour
{
    public  string targetName;
    public abstract void NewEvent(SimulationEvent simEvent);
    public abstract void ResetTarget();
    public TargetType targetType;
}