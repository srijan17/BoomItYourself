using UnityEngine;

public class MusicBoxController : MonoBehaviour
{
    public GameObject MageBody ;

    public float anglePerSecond;

    public bool isPlaying = false;

    public Quaternion initialRotation;
    private void Awake()
    {
        if (MageBody != null)
        {
            initialRotation = MageBody.transform.rotation;
        }
    }
    public void SetSpeed(float maxTimeDuration){
        
        anglePerSecond = (360f*2) / maxTimeDuration;
    }

    public void ResetMusicBox()
    {
        isPlaying = false;
        MageBody.transform.rotation = initialRotation;
    }
    public void PlayMusicBox()
    {
        isPlaying = true;
    }
    public void StopMusicBox()
    {
        isPlaying = false;
    }
    public void FixedUpdate()
    {
        if (MageBody != null && isPlaying)
        {
            // Rotate the MageBody around its Y-axis
            MageBody.transform.Rotate(Vector3.up, anglePerSecond * Time.fixedDeltaTime);
        }
    }
}