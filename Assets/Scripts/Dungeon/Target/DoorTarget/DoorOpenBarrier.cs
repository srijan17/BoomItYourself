using UnityEngine;

public class DoorOpenBarrier: MonoBehaviour
{

    private Renderer rend;
    private float InitialValue;
    private float FinalValue;
    public bool IsOpen { get; private set; }
    public float animationTime = 0.1f;

    private Coroutine routine;

    private void Start()
    {
         rend = GetComponent<Renderer>();
         InitialValue = rend.material.GetFloat("_Open");
         FinalValue = 1f; // Assuming the final value for the "_Open" property is 1
    }

    public void Toggle(bool open)
    {
        IsOpen = open;
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(Rotate());
    }

    private System.Collections.IEnumerator Rotate()
    {
        float startValue = IsOpen ? InitialValue : FinalValue;
        float endValue = IsOpen ? FinalValue : InitialValue;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(animationTime, 0.0001f);
            float value = Mathf.Lerp(startValue, endValue, t);
            rend.material.SetFloat("_Open", value);
            yield return null;
        }
        rend.material.SetFloat("_Open", endValue);
        routine = null;
    }
}