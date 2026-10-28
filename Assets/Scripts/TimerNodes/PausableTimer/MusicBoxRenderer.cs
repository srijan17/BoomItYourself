using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;
public class MusicBoxRenderer : MonoBehaviour
{
   

    [SerializeField] private ITimerNode timer;

    [Header("Start and complete node material Flash")]
    [SerializeField] private Material WireNodeOnMaterial;
    [SerializeField] private Material WireNodeOffMaterial;
    [SerializeField] private GameObject StartNode;
    [SerializeField] private GameObject EndNode;
    [SerializeField] private GameObject PauseNode;
    public AudioSource audioSource;

    [Header("Start and complete node Item Animation")]
    [SerializeField] private MusicBoxController MusicBox;

    [Header("Timer Value")]
    [SerializeField] private TMPro.TextMeshPro timerText;
    


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (timer == null)
        {
            timer = GetComponent<ITimerNode>();
        }
        else
        {
            Debug.LogWarning("Timer component is not assigned or found on the GameObject.");
            throw new System.Exception("Timer component is not assigned or found on the GameObject.");
        }

        //Register to the timer events
        timer.onNew.AddListener(OnNewTimerStart);
        timer.onStart.AddListener(OnTimerStart);
        timer.onPause.AddListener(OnTimerPause);
        timer.onComplete.AddListener(OnTimerComplete);

        MusicBox.StopMusicBox();
        MusicBox.SetSpeed(timer.maxDuration);
        MusicBox.ResetMusicBox();
        timerText.text = Mathf.CeilToInt(timer.maxDuration - timer.timeElapsed).ToString();
    }

    private void OnNewTimerStart()
    {
        // Handle the new timer start event
        Debug.Log("Music Box Timer New Start");
        MusicBox.ResetMusicBox();
        StopAllCoroutines();
        MusicBox.StopMusicBox();
        timerText.text = Mathf.CeilToInt(timer.maxDuration - timer.timeElapsed).ToString();
    }

    private void OnTimerStart()
    {
        // Handle the timer start event
        Debug.Log("Music Box Timer Started");
        audioSource.Play();
        MusicBox.PlayMusicBox();
        StartCoroutine(PulseMaterial(StartNode, WireNodeOnMaterial, 0.2f));

    }

    private void OnTimerPause()
    {
        // Handle the timer pause event
        audioSource.Pause();
        MusicBox.StopMusicBox();
        StartCoroutine(PulseMaterial(PauseNode, WireNodeOnMaterial, 0.2f));
        Debug.Log("Music Box Timer Paused");
    }

    private void OnTimerComplete()
    {
        // Handle the timer complete event
        Debug.Log("Music Box Timer Completed");
        StartCoroutine(PulseMaterial(EndNode, WireNodeOnMaterial, 0.2f));

        audioSource.Stop();
        MusicBox.StopMusicBox();
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

    void Update(){
        
        if(timer.timerState == TimerState.Running){
            timerText.text = Mathf.CeilToInt(timer.maxDuration - timer.timeElapsed).ToString();
        }
    }


    private IEnumerator TopHatBobbleAnimation(GameObject target, float bobbleHeight, float maxDuration)
    {
        Vector3 originalPosition = target.transform.localPosition;
        float elapsedTime = 0f;
        float bobbleFrequency = 3f; // Fast frequency
        float tiltAmount = 1f; // Tilt angle in degrees

        while (elapsedTime < maxDuration)
        {
            float cycleTime = (elapsedTime * bobbleFrequency) % 1f;
            
            // Up and down motion
            float verticalOffset = Mathf.Sin(cycleTime * Mathf.PI * 2f) * bobbleHeight;
            
            // Left and right motion
            float horizontalOffset = Mathf.Sin((cycleTime + 0.25f) * Mathf.PI * 2f) * bobbleHeight * (0.5f + ((elapsedTime/maxDuration)*5))*0.1f; // Adjust horizontal offset based on maxDuration
            
            // Tilt rotation
            float tilt = Mathf.Sin((cycleTime + 0.5f) * Mathf.PI * 2f) * (tiltAmount+ ((elapsedTime/maxDuration)*5)); // Adjust tilt based on maxDuration
            
            target.transform.localPosition = originalPosition + new Vector3(horizontalOffset, verticalOffset, 0);
            target.transform.localRotation = Quaternion.Euler(0, 0, tilt);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        target.transform.localPosition = originalPosition;
        target.transform.localRotation = Quaternion.identity;
    }
}