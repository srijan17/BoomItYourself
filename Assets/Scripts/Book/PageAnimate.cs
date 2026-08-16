using UnityEngine;
using System.Collections;
public class PageAnimate : MonoBehaviour
{
    public float AnimationDuration = 1f; // Duration of the page flip animation
    public Quaternion initialRotation;
    public Quaternion midRotation;
    public Quaternion targetRotation;
    public float startScaleX;
    public float midScaleX;
    public float targetScaleX;
    private void Awake()
    {
        initialRotation = transform.rotation;
        midRotation = initialRotation * Quaternion.Euler(0f, 90f, 0f); // Rotate 90 degrees around the Y-axis
        targetRotation =  initialRotation * Quaternion.Euler(0f, 180f, 0f);  ;
        startScaleX = transform.localScale.x;
        midScaleX = 0f; // Set the mid scale to 0 for the flip effect
        targetScaleX = startScaleX;
    }
    public void SetSpeed(float duration)
    {
        AnimationDuration = duration;
    }
    public void AnimatePageFlip()
    {
        // Start the page flip animation
        StartCoroutine(PageFlipCoroutine());
    }

    private IEnumerator PageFlipCoroutine()
    {
        float elapsedTime = 0f;
       // Flip the scale on the X-axis
        while (elapsedTime < AnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / AnimationDuration);
            if(elapsedTime < AnimationDuration / 2f)
            {
                // First half of the animation: rotate to midRotation
                transform.rotation = Quaternion.Slerp(initialRotation, midRotation, t * 2f);
            }
            else
            {
                // Second half of the animation: rotate to targetRotation
                transform.rotation = Quaternion.Slerp(midRotation, targetRotation, (t - 0.5f) * 2f);
            }
            // transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t); // This line is redundant and should be removed
            if(elapsedTime < AnimationDuration / 2f)
            {
                transform.localScale = new Vector3(Mathf.Lerp(startScaleX, midScaleX, t * 2f), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Lerp(midScaleX, targetScaleX, (t - 0.5f) * 2f), transform.localScale.y, transform.localScale.z);
            }
            yield return null;
        }

        // Ensure the final rotation is set
        transform.rotation = targetRotation;
        gameObject.SetActive(false); // Deactivate the page after the flip animation is complete
    }


    //Reverse the page flip animation
    public void AnimatePageFlipBack()
    {
        // Start the page flip animation
        StartCoroutine(PageFlipBackCoroutine());
    }
    private IEnumerator PageFlipBackCoroutine()
    {
        float elapsedTime = 0f;
       // Flip the scale on the X-axis
        while (elapsedTime < AnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / AnimationDuration);
            if(elapsedTime < AnimationDuration / 2f)
            {
                // First half of the animation: rotate to midRotation
                transform.rotation = Quaternion.Slerp(targetRotation, midRotation, t * 2f);
            }
            else
            {
                // Second half of the animation: rotate to targetRotation
                transform.rotation = Quaternion.Slerp(midRotation, initialRotation, (t - 0.5f) * 2f);
            }
            // transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t); // This line is redundant and should be removed
            if(elapsedTime < AnimationDuration / 2f)
            {
                transform.localScale = new Vector3(Mathf.Lerp(targetScaleX, midScaleX, t * 2f), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(Mathf.Lerp(midScaleX, startScaleX, (t - 0.5f) * 2f), transform.localScale.y, transform.localScale.z);
            }
            yield return null;
        }

        // Ensure the final rotation is set
        transform.rotation = initialRotation;
    }
}