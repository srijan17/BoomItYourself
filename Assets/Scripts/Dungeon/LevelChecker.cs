using UnityEngine;

public class LevelChecker : MonoBehaviour
{
    public GameObject BombTarget;
    public GameObject Hero;

    public SimulationManager simulationManager;
    public void Awake(){
        simulationManager.onNewEvent.AddListener(OnSimulationEvent);
    }
    

    public void OnSimulationEvent(SimulationEvent simEvent)
    {
        // Check if the event is relevant to the level completion
        if (simEvent.EventType == SimulationEventType.Explode)
        {
            // Implement your logic to check if all necessary events have occurred
            // For example, you can check if all required targets have been triggered
            CheckLevelCompletion();
        }
        
    }
    public void CheckLevelCompletion()
    {

        Debug.Log("Checking level completion...");
        Debug.Log($"BombTarget: {BombTarget.transform.position}, Hero: {Hero.transform.position}");

        Debug.Log($"Simulation Elapsed Time: {simulationManager.simulationElapsedTime}");
        // Implement your level completion logic here
        //Check Hero Distance and elapsed Time and target time 
        if(BombTarget!=null && Hero!=null){
            float distance = Vector3.Distance(BombTarget.transform.position, Hero.transform.position);
            if(distance<1f && simulationManager.simulationElapsedTime >= simulationManager.TargetTime){
                Debug.Log("Level completed!");
            }
        }
        else{
            Debug.LogWarning("BombTarget or Hero is not assigned in LevelChecker.");
        }
        // Debug.Log("Level completed!");


    }
}