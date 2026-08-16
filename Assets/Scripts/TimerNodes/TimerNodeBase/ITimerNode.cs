
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
public enum TimerState
{
    Idle,
    Running,
    Paused,
    Completed
}

public abstract class ITimerNode : MonoBehaviour
{
    public float maxDuration;
    public float timeElapsed;
    public float timeSpan;

    public abstract void Reset();
    public TimerState timerState;
    public UnityEvent onNew = new UnityEvent();
    public UnityEvent onStart = new UnityEvent();
    public UnityEvent onComplete = new UnityEvent();
    public UnityEvent onPause = new UnityEvent();

    public abstract void StartTimer();
    public abstract void PauseTimer();
}