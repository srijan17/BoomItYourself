using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;
public class RiceCookerRenderer3d : MonoBehaviour
{
   

    [SerializeField] private ITimerNode riceCookerTimer;

    [Header("Start and complete node material Flash")]
    [SerializeField] private Material WireNodeOnMaterial;
    [SerializeField] private Material WireNodeOffMaterial;
    [SerializeField] private GameObject StartNode;
    [SerializeField] private GameObject EndNode;
    // [SerializeField] private GameObject LightNode;

    [Header("Start and complete node Item Animation")]
    [SerializeField] private GameObject CookerHolder;
    [SerializeField] private GameObject CookerSteam;

    [SerializeField] private float CookerMaxScale = 1.0f;
    [SerializeField] private float reduceRate = 0.1f; // Rate at which the flame reduces in size
    [SerializeField] private float minAngle = 0.1f; // Rate at which the flame reduces in size
    [SerializeField] private float maxAngle = 0.1f; // Rate at which the flame reduces in size
    [SerializeField] private float wobbleRate = 0.1f; // Rate at which the flame reduces in size
    public bool isPlaying = false;
    public AudioSource audioSource;
    
    
    // [SerializeField] private GameObject ItemHolder;
    // [SerializeField] private Sprite IdleImage;
    // [SerializeField] private List<Sprite> StartImage;
    // [SerializeField] private Sprite EndImage;

    [Header("Timer Value")]
    [SerializeField] private TMPro.TextMeshPro timerText;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (riceCookerTimer == null)
        {
            riceCookerTimer = GetComponent<ITimerNode>();
        }
        else
        {
            Debug.LogWarning("RiceCookerTimer component is not assigned or found on the GameObject.");
            throw new System.Exception("RiceCookerTimer component is not assigned or found on the GameObject.");
        }

        //Register to the timer events
        riceCookerTimer.onNew.AddListener(OnNewTimerStart);
        riceCookerTimer.onStart.AddListener(OnTimerStart);
        riceCookerTimer.onPause.AddListener(OnTimerPause);
        riceCookerTimer.onComplete.AddListener(OnTimerComplete);
        CookerHolder.SetActive(true);
        CookerSteam.SetActive(false);
        CookerHolder.transform.rotation = Quaternion.Euler(0, 0, 0);
        timerText.text = Mathf.CeilToInt(riceCookerTimer.maxDuration).ToString();
    }

    



    public void ResetRiceCooker()
    {
        isPlaying = false;
        CookerHolder.SetActive(true);
        CookerSteam.SetActive(false);
        CookerHolder.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void OnTimerStart()
    {
        audioSource.Play();
        // Handle the timer start event
        Debug.Log("Rice Cooker Timer Started");
        isPlaying = true;
        StartCoroutine(PulseMaterial(StartNode, WireNodeOnMaterial, 0.2f));
  
    }

    private void OnTimerPause()
    {
        audioSource.Pause();
        // Handle the timer pause event
        Debug.Log("Rice Cooker Timer Paused");
        isPlaying = false;

    }

    private void OnTimerComplete()
    {
        CookerHolder.transform.localRotation = Quaternion.Euler(0, 0, 0);
        isPlaying = false;
        audioSource.Stop();
        // Handle the timer complete event
        Debug.Log("Rice Cooker Timer Completed");
        StartCoroutine(PulseMaterial(EndNode, WireNodeOnMaterial, 0.2f));
        timerText.text = Mathf.CeilToInt(riceCookerTimer.maxDuration - riceCookerTimer.timeElapsed).ToString();
       
    }


    private void OnNewTimerStart()
    {
        // Handle the new timer start event
        Debug.Log("Rice Cooker Timer New Start");
        StopAllCoroutines();
        isPlaying = false;
        timerText.text = Mathf.CeilToInt(riceCookerTimer.maxDuration - riceCookerTimer.timeElapsed).ToString();
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
          if(riceCookerTimer.timerState == TimerState.Running){
            timerText.text = Mathf.CeilToInt(riceCookerTimer.maxDuration - riceCookerTimer.timeElapsed).ToString();
        }
        
        if ( isPlaying)
        {
            // Reduce the candle flame size over time
            float t = Mathf.PingPong(Time.time * wobbleRate, 1f);
            float angle = Mathf.Lerp(minAngle, maxAngle, t);
            CookerHolder.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        if(isPlaying){
            CookerSteam.SetActive(true);
        }
        else{
            CookerSteam.SetActive(false);
        }
    }

}