using UnityEngine;
using System.Collections;
public class Blink : MonoBehaviour
{
    [SerializeField] private float blinkDuration = 0.1f;

    private Renderer objectRenderer;

    private float timeElapsed = 0f;
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("No Renderer component found on this GameObject.");
        }
    }




    void OnDestroy()
    {
        StopAllCoroutines();
    }

    void Update()
    {
       //Constantly Keep blinking the object
        if (objectRenderer != null)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= blinkDuration)
            {
                objectRenderer.enabled = !objectRenderer.enabled; // Toggle visibility
                timeElapsed = 0f; // Reset the timer
            }
        }
       
    }

}