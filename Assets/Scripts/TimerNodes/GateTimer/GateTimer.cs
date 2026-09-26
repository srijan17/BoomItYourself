using UnityEngine;
using System.Collections;
using UnityEngine.Events;

//Need to rename to general timer
public class GateTimer : ITimerNode
{
   
    public bool isGateOpen = false;
    public bool isGateSwitching = false;
    public override void StartTimer()
    {   
        if(isGateOpen)
        {
          
        if (timerState == TimerState.Running)
            return;


        if(timerState == TimerState.Paused)
        {
            timerState = TimerState.Running;
            onStart.Invoke();
            StartCoroutine(TimerCoroutine());
            return;
        }
        timeElapsed = 0f;
        timerState = TimerState.Running;
        // ChangeMaterial(onStartMaterial);
        onStart.Invoke();
        StartCoroutine(TimerCoroutine());
    }
    }

       public override void Reset()
    {
        isGateSwitching= false;
        isGateOpen = false;
        StopAllCoroutines();
        timeElapsed = 0f;
        timerState = TimerState.Idle;
        onNew.Invoke();
    }


    public override void PauseTimer()
    {
        isGateSwitching=true;
        StartCoroutine(GateToggleCoroutine());
    }

    private IEnumerator GateToggleCoroutine()
    {
        yield return new WaitForSeconds(0.3f); // Wait for 0.3 seconds before toggling the gate
        isGateOpen = !isGateOpen;
        isGateSwitching = false;
        Debug.Log("Gate is now " + (isGateOpen ? "open" : "closed") + ".");
    }

    private IEnumerator TimerCoroutine()
    {
        while (timeElapsed < maxDuration)
        {

            //may use timespan to increment timeElapsed in a different way if needed
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timerState = TimerState.Completed;
        completed();
    }

    public void completed()
    {
        
        
        onComplete.Invoke();
   
    }

  
}