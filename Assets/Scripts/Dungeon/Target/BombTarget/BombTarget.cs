using UnityEngine;

public class BombTarget : ITarget
{
    public override string targetName { get; set; }

    public GameObject bomb; 

    public GameObject explosionEffect; 
    private void Awake()
    {
        // Initialize the target name or any other necessary properties
        targetName = "BombTarget"; // Example target name
        SimulationManager.Instance.onNewEvent.AddListener(NewEvent);
    }


    public override void NewEvent(SimulationEvent simEvent)
    {
        if (simEvent.TargetName == targetName && simEvent.EventType == SimulationEventType.Explode)
        {
            // Instantiate the bomb prefab at the target's position
            if(bomb!=null){
                bomb.SetActive(false); // Hide the bomb object
            }
            if(explosionEffect!=null){
                explosionEffect.SetActive(true); // Show the explosion effect
            }
        }
    }

    public override void ResetTarget()
    {
        if(bomb!=null){
            bomb.SetActive(true); // Show the bomb object
        }
        if(explosionEffect!=null){
            explosionEffect.SetActive(false); // Hide the explosion effect
        }
        // Implement the behavior to reset the target
    }
}