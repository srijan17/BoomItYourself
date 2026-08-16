using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public enum SimulationEventType{
    StartSimulation,
    Explode,
    DoorOpen,
    DoorClose,
    Toggle,
    TurnOn,
    TurnOff,
}

public struct SimulationEvent{
    public string TargetName;
    public SimulationEventType EventType;
    public float EventTime;
}
public class SimulationManager : MonoBehaviour
{
    public float simulationTimeScale = 1f;

    public float simulationElapsedTime = 0f;
    public bool isSimulationRunning = false;

    [SerializeField] private float targetTime = 5f;

    public float TargetTime => targetTime;
    
    
    [SerializeField] private List<SimulationEvent> simulationEvents = new List<SimulationEvent>();

    public List<SimulationEvent> SimulationEvents => simulationEvents;

    public UnityEvent<SimulationEvent> onNewEvent = new UnityEvent<SimulationEvent>();


    public static SimulationManager Instance { get; private set; }

    private void Awake()
    {
        
        if (Instance != null && Instance != this)
        {
            // Destroy(gameObject);
            return;
        }

        Instance = this;
        ResetSimulation();
        // DontDestroyOnLoad(gameObject);
    }

    public void ResetSimulation(){
        isSimulationRunning = false;
        simulationElapsedTime = 0f;
        ClearSimulationEvents();
    }

    public void AddSimulationEvent(string targetName, SimulationEventType eventType, float eventTime)
    {
        SimulationEvent newEvent = new SimulationEvent
        {
            TargetName = targetName,
            EventType = eventType,
            EventTime = eventTime
        };
        simulationEvents.Add(newEvent);
        onNewEvent.Invoke(newEvent);
        
        if(eventType==SimulationEventType.StartSimulation){
            isSimulationRunning = true;
        }
    }

    public void ClearSimulationEvents()
    {
        simulationEvents.Clear();
    }

    public void Update(){
        if(isSimulationRunning){
            simulationElapsedTime += Time.deltaTime * simulationTimeScale;
            
        }
    }

}