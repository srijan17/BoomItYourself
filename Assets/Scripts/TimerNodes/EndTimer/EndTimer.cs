using UnityEngine;
using System.Collections;
using UnityEngine.Events;
public class EndTimer : ITimerNode
{

   [SerializeField] private SimulationEventType eventType;
   [SerializeField] private ITarget target;
    public override void StartTimer()
    {
       completed();
    }

 
  public override void PauseTimer()
    {
    }
    public void completed()
    {
       
       // Add the simulation event to the SimulationManager
       SimulationManager.Instance.AddSimulationEvent(target.targetName, eventType, Time.time);
        onComplete.Invoke();
       
    }

    public override void Reset()
    {
        StopAllCoroutines();
        timeElapsed = 0f;
        timerState = TimerState.Idle;
        onNew.Invoke();
    }

   
}