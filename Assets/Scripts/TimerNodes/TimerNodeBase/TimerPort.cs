using UnityEngine;
using UnityEngine.Events;

public enum TimerPortType
{
    Start,
    Complete,
    Pause
}

[RequireComponent(typeof(WireNode))]
public class TimerPort : MonoBehaviour
{
    [SerializeField] private TimerPortType portType;

    [Tooltip("Automatically finds a timer on a parent if empty.")]
    [SerializeField] private ITimerNode timer;

    public TimerPortType PortType => portType;
    public ITimerNode Timer => timer;

    private void Awake()
    {
        if (timer == null)
        {
            timer = GetComponentInParent<ITimerNode>();
        }

        if (timer == null)
        {
            Debug.LogError(
                $"{name} could not find an ITimerNode in its parents.",
                this
            );
        }
    }

    /// <summary>
    /// Defines which port combinations are currently allowed.
    /// </summary>
    public bool CanConnectTo(TimerPort target)
    {
        if (target == null || timer == null || target.timer == null)
            return false;

        // Do not wire a timer to itself.
        if (timer == target.timer)
            return false;

        /*
         * Initial rule:
         *
         * Complete event -> Start function
         */
        return portType == TimerPortType.Complete &&
               (target.portType == TimerPortType.Start || target.portType == TimerPortType.Pause);
    }

    /// <summary>
    /// Returns the UnityEvent produced by this port.
    /// </summary>
    public UnityEvent GetSourceEvent()
    {
        if (timer == null)
            return null;

        switch (portType)
        {
            case TimerPortType.Start:
                return timer.onStart;

            case TimerPortType.Complete:
                return timer.onComplete;

            case TimerPortType.Pause:
                return timer.onPause;

            default:
                return null;
        }
    }

    /// <summary>
    /// Returns the timer function activated by this port.
    /// </summary>
    public UnityAction GetTargetAction()
    {
        if (timer == null)
            return null;

        switch (portType)
        {
            case TimerPortType.Start:
                return timer.StartTimer;

            case TimerPortType.Pause:
                return timer.PauseTimer;

            /*
             * Complete is currently an output-only port.
             * It does not have a target action.
             */
            case TimerPortType.Complete:
                return null;

            default:
                return null;
        }
    }
}