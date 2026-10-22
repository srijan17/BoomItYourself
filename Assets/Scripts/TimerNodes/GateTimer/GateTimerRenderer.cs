using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;
public class GateTimerRenderer : MonoBehaviour
{
   
   [SerializeField] private GateTimer gateTimer;


    [Header("Start and complete node material Flash")]
    [SerializeField] private Material WireNodeOnMaterial;
    [SerializeField] private Material WireNodeOffMaterial;
    [SerializeField] private GameObject StartNode;
    [SerializeField] private GameObject EndNode;
    [SerializeField] private GameObject LightNode;
    [SerializeField] private GameObject YellowLight;
    [SerializeField] private GameObject Car;
    [SerializeField] private Transform CarInitialPosition;
    [SerializeField] private Transform FinalPosition;
    [Header("Start and complete node Item Animation")]
    [SerializeField] private GameObject GreenLight;
    [SerializeField] private GameObject RedLight;

    [SerializeField] private bool isGateOpen = false;


    [Header("Timer Value")]
    [SerializeField] private TMPro.TMP_Text  timerText;
    private void Awake()
    {
        gateTimer = GetComponent<GateTimer>();
        if (gateTimer == null)
        {
            Debug.LogWarning("GateRenderer3d component is not assigned or found on the GameObject.");
            throw new System.Exception("GateRenderer3d component is not assigned or found on the GameObject.");
        }

        //Register to the timer events
        gateTimer.onNew.AddListener(OnNewTimerStart);
        gateTimer.onStart.AddListener(OnTimerStart);
        gateTimer.onPause.AddListener(OnTimerPause);
        gateTimer.onComplete.AddListener(OnTimerComplete);

        GreenLight.SetActive(false);
        RedLight.SetActive(true);
        timerText.text = Mathf.CeilToInt(gateTimer.maxDuration).ToString();
    }


    private void OnNewTimerStart()
    {
        // Handle the new timer start event
        Debug.Log("Gate Renderer 3D Timer New Start");
        StopAllCoroutines();
        GreenLight.SetActive(false);
        RedLight.SetActive(true);
        timerText.text = Mathf.CeilToInt(gateTimer.maxDuration - gateTimer.timeElapsed).ToString();
    }

    private void OnTimerStart()
    {
        // Handle the timer start event
        Debug.Log("Gate Renderer 3D Timer Started");
        StartCoroutine(PulseMaterial(StartNode, WireNodeOnMaterial, 0.2f));


    }

    private void OnTimerPause()
    {
        // Handle the timer pause event
        Debug.Log($"Gate Renderer 3D Timer Paused {gameObject.name}");

    }

    private void OnTimerComplete()
    {
        // Handle the timer complete event
        Debug.Log("Gate Renderer 3D  Timer Completed");
        StartCoroutine(PulseMaterial(EndNode, WireNodeOnMaterial, 0.2f));
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
        if(gateTimer.isGateOpen && gateTimer.isGateOpen!= isGateOpen){
            GreenLight.SetActive(true);
            RedLight.SetActive(false);
            isGateOpen = true;
        }
        if(!gateTimer.isGateOpen && gateTimer.isGateOpen != isGateOpen){
            GreenLight.SetActive(false);
            RedLight.SetActive(true);
            isGateOpen = false;
        }

        if(gateTimer.isGateSwitching){
            YellowLight.SetActive(true);
        } else{
            YellowLight.SetActive(false);
        }


        if(gateTimer.timerState == TimerState.Running){
            //Lerp Car position from initial to final based on max Duration and time elapsed
            float t = gateTimer.timeElapsed / gateTimer.maxDuration;
            Car.transform.position = Vector3.Lerp(CarInitialPosition.position, FinalPosition.position, t);
            timerText.text = Mathf.CeilToInt(gateTimer.maxDuration - gateTimer.timeElapsed).ToString();
        }
    }


}