using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class BeginTimer : ITimerNode
{

   
    public override void StartTimer()
    {
        SimulationManager.Instance.AddSimulationEvent("Start", SimulationEventType.StartSimulation, Time.time);
       completed();
    }

 
    public override void PauseTimer()
    {
    }

    public void completed()
    {
       
        
        onComplete.Invoke();
       
    }

    public override void Reset()
    {
        timeElapsed = 0f;
        timerState = TimerState.Idle;
        onNew.Invoke();
    }

   
}