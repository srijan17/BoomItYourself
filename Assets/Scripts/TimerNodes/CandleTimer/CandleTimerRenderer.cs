using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;
public class CandleRenderer : MonoBehaviour
{
   

    [SerializeField] private ITimerNode candleTimer;

    [Header("Start and complete node material Flash")]
    [SerializeField] private Material WireNodeOnMaterial;
    [SerializeField] private Material WireNodeOffMaterial;
    [SerializeField] private GameObject StartNode;
    [SerializeField] private GameObject EndNode;
    [SerializeField] private GameObject LightNode;

    [Header("Start and complete node Item Animation")]
    [SerializeField] private GameObject CandleHolder;
    [SerializeField] private GameObject CandleFlame;

    [SerializeField] private float CandleMaxScale = 1.0f;
    [SerializeField] private float reduceRate = 0.1f; // Rate at which the flame reduces in size
    public bool isPlaying = false;

    
    
    // [SerializeField] private GameObject ItemHolder;
    // [SerializeField] private Sprite IdleImage;
    // [SerializeField] private List<Sprite> StartImage;
    // [SerializeField] private Sprite EndImage;

    [Header("Timer Value")]
    [SerializeField] private TMPro.TextMeshPro timerText;

    private void Awake()
    {
        if (candleTimer == null)
        {
            candleTimer = GetComponent<ITimerNode>();
        }
        else
        {
            Debug.LogWarning("CandleTimer component is not assigned or found on the GameObject.");
            throw new System.Exception("CandleTimer component is not assigned or found on the GameObject.");
        }

        //Register to the timer events
        candleTimer.onNew.AddListener(OnNewTimerStart);
        candleTimer.onStart.AddListener(OnTimerStart);
        candleTimer.onPause.AddListener(OnTimerPause);
        candleTimer.onComplete.AddListener(OnTimerComplete);
        SetSpeed(candleTimer.maxDuration);
        CandleHolder.SetActive(true);
        CandleFlame.SetActive(false);
        CandleHolder.transform.localScale = Vector3.one * CandleMaxScale;
        timerText.text = Mathf.CeilToInt(candleTimer.maxDuration).ToString();
    }

    

    public void SetSpeed(float maxTimeDuration){
            reduceRate = (CandleMaxScale- 0.1f) / maxTimeDuration;    
    }

    public void ResetCandle()
    {
        SetSpeed(candleTimer.maxDuration);
        isPlaying = false;
        CandleHolder.transform.localScale = Vector3.one * CandleMaxScale;
    }

    private void OnTimerStart()
    {

        // Handle the timer start event
        Debug.Log("Candle Timer Started");
        isPlaying = true;
        StartCoroutine(PulseMaterial(StartNode, WireNodeOnMaterial, 0.2f));
        // ItemHolder.GetComponent<SpriteRenderer>().sprite = getStartImageForTime(candleTimer.timeElapsed);
        LightNode.SetActive(true);

    }

    private void OnTimerPause()
    {
        // Handle the timer pause event
        Debug.Log("Candle Timer Paused");
        isPlaying = false;
        LightNode.SetActive(false);

    }

    private void OnTimerComplete()
    {
        isPlaying = false;
        // Handle the timer complete event
        Debug.Log("Candle Timer Completed");
        StartCoroutine(PulseMaterial(EndNode, WireNodeOnMaterial, 0.2f));
        timerText.text = Mathf.CeilToInt(candleTimer.maxDuration - candleTimer.timeElapsed).ToString();
        // ItemHolder.GetComponent<SpriteRenderer>().sprite = EndImage;
        LightNode.SetActive(false);
    }


    private void OnNewTimerStart()
    {
        CandleHolder.transform.localScale = Vector3.one * CandleMaxScale;

        // Handle the new timer start event
        Debug.Log("Candle Timer New Start");
        StopAllCoroutines();
        isPlaying = false;
        // ItemHolder.GetComponent<SpriteRenderer>().sprite = IdleImage;
        timerText.text = Mathf.CeilToInt(candleTimer.maxDuration - candleTimer.timeElapsed).ToString();
        LightNode.SetActive(false);
    }

    private IEnumerator PulseMaterial(GameObject target, Material material, float duration)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material originalMaterial = renderer.material;
            renderer.material = material;

            yield return new WaitForSeconds(duration);

            renderer.material = originalMaterial;
        }
    }

    public void FixedUpdate()
    {
          if(candleTimer.timerState == TimerState.Running){
            timerText.text = Mathf.CeilToInt(candleTimer.maxDuration - candleTimer.timeElapsed).ToString();
        }
        
        if ( isPlaying)
        {
            // Reduce the candle flame size over time
            float newScale = CandleHolder.transform.localScale.y - reduceRate * Time.fixedDeltaTime;
            newScale = Mathf.Max(newScale, 0.1f); // Ensure the flame doesn't disappear completely
            CandleHolder.transform.localScale = new Vector3(1, newScale, 1);
        }

        if(isPlaying){
            CandleFlame.SetActive(true);
        }
        else{
            CandleFlame.SetActive(false);
        }
    }

    // private Sprite getStartImageForTime(float timeElapsed)
    // {
    //     int index = Mathf.FloorToInt((timeElapsed / candleTimer.maxDuration) * StartImage.Count);
    //     index = Mathf.Clamp(index, 0, StartImage.Count - 1);
    //     return StartImage[index];
    // }
}