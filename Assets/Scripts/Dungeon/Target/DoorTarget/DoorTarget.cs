using System.Collections;
using UnityEngine;

public class DoorTarget : ITarget
{

    public Junction sourceJunction;

    public Junction targetJunction;

    public string routeId; // The route ID associated with this door

    public bool initialState = false; // The initial state of the door (open or closed)
    public bool isDoorOpen = false; // Track the door's open/closed state

    public DoorOpenBarrier doorOpen; // Reference to the DoorOpen script for animation

    private void Awake()
    {
        // Initialize the target name or any other necessary properties
        SimulationManager.Instance.onNewEvent.AddListener(NewEvent);

        //Open/Close the junction route based on the door's initial state
        if (sourceJunction != null && targetJunction != null)
        {
            isDoorOpen = initialState;
            doorOpen.Toggle(isDoorOpen);
            sourceJunction.ToggleRoute(routeId, isDoorOpen);
        }
    }


    public override void NewEvent(SimulationEvent simEvent)
    {
        if (simEvent.TargetName == targetName && simEvent.EventType == SimulationEventType.DoorToggle)
        {
           DoorToggle();
        }
    }

    private void DoorToggle()
    {
  
            //Implement Later: Add animation or sound effects for door open and close
            // door.SetActive(!door.activeSelf); // Toggle the door's active state
            isDoorOpen = !isDoorOpen; // Update the door's open/closed state
            if (doorOpen != null)
            {
                doorOpen.Toggle(isDoorOpen); // Animate the door
            }
            if (sourceJunction != null && targetJunction != null)
            {
                sourceJunction.ToggleRoute(routeId, isDoorOpen);
            }
    }

    public override void ResetTarget()
    {
       
        isDoorOpen = initialState; // Update the door's open/closed state based on the initial state
        sourceJunction.ToggleRoute(routeId, initialState);
        if(doorOpen != null)
        {
            doorOpen.Toggle(isDoorOpen); // Animate the door to its reset state
        }
        // Implement the behavior to reset the target
    }

}