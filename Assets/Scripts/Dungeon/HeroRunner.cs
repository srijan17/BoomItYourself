using UnityEngine;
using System.Collections;
using UnityEngine.Splines;
using Unity.Collections;

public class HeroRunner : MonoBehaviour
{
      [Header("Target")]
    [Tooltip("Human-readable knot number. 1 = first knot, 3 = third knot.")]
    [SerializeField, Min(2)]
    private int targetKnotNumber = 3;

    [Tooltip("Seconds after starting when the hero must reach the target knot.")]
    [SerializeField, Min(0.01f)]
    private float targetArrivalTime;

    private float routineStartTime = -9999f;

    [Header("Startup")]
    [SerializeField]
    private bool playOnStart = true;
    private Coroutine startRoutine;
    public float CalculatedSpeed { get; private set; }

    private SplineAnimate splineAnimate;

    private void Awake()
    {
        routineStartTime = -9999f;
        splineAnimate = GetComponent<SplineAnimate>();

        // This script starts the animation after calculating the speed.
        splineAnimate.PlayOnAwake = false;
        // targetArrivalTime = DungeonSimulation.GetTargetTime();
   
    }

     private void Start()
    {
        if (playOnStart)
        {
            StartJourney();
        }
    }


     public void StartJourney()
{
    if (startRoutine != null)
    {
        StopCoroutine(startRoutine);
    }

    startRoutine = StartCoroutine(StartJourneyRoutine());
    routineStartTime = Time.time;
}



private IEnumerator StartJourneyRoutine()
{

    // Important when this object was activated this frame.
    yield return null;

    if (splineAnimate == null)
    {
        splineAnimate = GetComponent<SplineAnimate>();
    }

    splineAnimate.enabled = true;

    if (splineAnimate.Container == null)
    {
        Debug.LogError($"{name}: Spline Animate has no Spline Container.");
        yield break;
    }

    if (targetArrivalTime <= 0f)
    {
        Debug.LogError($"{name}: Target arrival time must be greater than zero.");
        yield break;
    }

    Spline spline = splineAnimate.Container.Spline;

    if (spline == null || spline.Count < 2)
    {
        Debug.LogError($"{name}: The spline needs at least two knots.");
        yield break;
    }

    int targetKnotIndex = Mathf.Clamp(
        targetKnotNumber - 1,
        1,
        spline.Count - 1
    );

    float distanceToTarget =
        GetWorldDistanceToKnot(spline, targetKnotIndex);

    CalculatedSpeed = distanceToTarget / targetArrivalTime;

    // Set method before setting MaxSpeed.
    splineAnimate.AnimationMethod = SplineAnimate.Method.Speed;
    splineAnimate.Easing = SplineAnimate.EasingMode.None;
    splineAnimate.Loop = SplineAnimate.LoopMode.Once;
    splineAnimate.StartOffset = 0f;
    splineAnimate.MaxSpeed = CalculatedSpeed;

    // Reset first, then explicitly play.
    splineAnimate.Restart(false);
    splineAnimate.Play();

    Debug.Log(
        $"{name}: active={gameObject.activeInHierarchy}, " +
        $"component enabled={splineAnimate.enabled}, " +
        $"playing={splineAnimate.IsPlaying}, " +
        $"timeScale={Time.timeScale}, " +
        $"distance={distanceToTarget:F2}, " +
        $"speed={CalculatedSpeed:F2}, " +
        $"duration={splineAnimate.Duration:F2}"
    );

    startRoutine = null;
}

    public void Reset(){
        splineAnimate.Restart(false);
        splineAnimate.Pause();
        StopCoroutine(startRoutine);
        routineStartTime = -9999f;
         }
  private float GetWorldDistanceToKnot(
        Spline spline,
        int targetKnotIndex)
    {
        // NativeSpline applies the SplineContainer's transform,
        // so the calculated distance is in world space.
        NativeSpline worldSpline = new NativeSpline(
            spline,
            splineAnimate.Container.transform.localToWorldMatrix,
            Allocator.Temp
        );

        float distance = 0f;

        // Curve 0 connects knot 1 to knot 2.
        // Curve 1 connects knot 2 to knot 3.
        //
        // Therefore, reaching knot 3 means adding curves 0 and 1.
        for (int curveIndex = 0;
             curveIndex < targetKnotIndex;
             curveIndex++)
        {
            distance += worldSpline.GetCurveLength(curveIndex);
        }

        worldSpline.Dispose();

        return distance;
    }
}