using UnityEngine;

public class HeroTarget : ITarget
{
    public override string targetName { get; set; }
    public bool isDead = false;

    public HeroRunner2 heroRunner; // Reference to the HeroRunner script

    private void Start()
    {
        heroRunner = GetComponent<HeroRunner2>(); // Get the HeroRunner component attached to the same GameObject
        if (heroRunner == null)
        {
            Debug.LogError("HeroRunner component not found on the GameObject.");
        }
       // Initialize the target name or any other necessary properties
        targetName = "HeroTarget"; // Example target name
        SimulationManager.Instance.onNewEvent.AddListener(NewEvent);
    }


    public override void NewEvent(SimulationEvent simEvent)
    {
        if ( simEvent.EventType == SimulationEventType.StartSimulation)
        {
            // heroRunner is the reference to the HeroRunner script, you can call its methods or access its properties here
            if(heroRunner!=null){
                heroRunner.StartJourney(); // Show the hero runner object
            }
        }
     
    }

    public override void ResetTarget()
    {
        if(heroRunner!=null){
            heroRunner.Reset(); // Hide the hero runner object
        }
        // Implement the behavior to reset the target
    }

    public void KillHero()
    {
        if(isDead)
            return;
        isDead = true;
        if(heroRunner!=null){
         SimulationManager.Instance.AddSimulationEvent(heroRunner.gameObject.name, SimulationEventType.HeroDied, SimulationManager.Instance.simulationElapsedTime);

        }
    }

    public void Escape()
    {
        if(isDead)
            return;
        if(heroRunner!=null){
         SimulationManager.Instance.AddSimulationEvent(heroRunner.gameObject.name, SimulationEventType.HeroEscaped, SimulationManager.Instance.simulationElapsedTime);

        }
    }
}