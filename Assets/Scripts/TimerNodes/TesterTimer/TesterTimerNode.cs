using System.Collections;
using UnityEngine;

public class TesterTimer : ITimerNode
{
   
    [Header("Reset")]
    [SerializeField, Min(0f)]
    private float resetCooldown = 0.1f;

    private Coroutine timerCoroutine;
    private Coroutine resetCoroutine;



    public override void StartTimer()
    {
        // Starting an already-running timer does nothing.
        if (timerState == TimerState.Running)
            return;

        // Do not allow restarting during the completion cooldown.
        if (resetCoroutine != null)
            return;

        timerState = TimerState.Running;
        onStart.Invoke();

        if (timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(TimerCoroutine());
        }
    }

    public override void PauseTimer()
    {
        if (timerState != TimerState.Running)
            return;

        timerState = TimerState.Paused;
        onPause.Invoke();

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    private IEnumerator TimerCoroutine()
    {
        while (timeElapsed < maxDuration)
        {
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        timerCoroutine = null;
        timeElapsed = maxDuration;
        timerState = TimerState.Completed;
        onComplete.Invoke();

    }

  

    private IEnumerator ResetAfterCooldown()
    {
        yield return new WaitForSeconds(resetCooldown);

        timeElapsed = 0f;

        // Replace Idle with whatever your initial/ready enum value is.
        timerState = TimerState.Idle;
        onStart.Invoke(); // Optionally invoke onStart to indicate readiness for a new cycle.

        resetCoroutine = null;
    }

    public override void Reset()
    {
        StopAllCoroutines();
        timeElapsed = 0f;
        timerState = TimerState.Idle;
        onNew.Invoke();
    }
  
}