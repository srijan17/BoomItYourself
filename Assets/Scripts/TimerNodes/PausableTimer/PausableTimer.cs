using UnityEngine;
using System.Collections;
using UnityEngine.Events;

//Need to rename to general timer
public class PausableTimer : ITimerNode
{
    public override void StartTimer()
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
        timerState = TimerState.Running;
        // ChangeMaterial(onStartMaterial);
        onStart.Invoke();
        StartCoroutine(TimerCoroutine());
    }

  
    public override void UpdateMaxDuration(float maxDuration)
    {
        this.maxDuration = maxDuration;
        Reset();
    }
    public override void Reset()
    {
        StopAllCoroutines();
        timeElapsed = 0f;
        timerState = TimerState.Idle;
        onNew.Invoke();
    }


    public override void PauseTimer()
    {
        if (timerState != TimerState.Running)
            return;

        timerState = TimerState.Paused;

        onPause.Invoke();
        StopAllCoroutines();
    }

    private IEnumerator TimerCoroutine()
    {
        while (timeElapsed < maxDuration)
        {

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