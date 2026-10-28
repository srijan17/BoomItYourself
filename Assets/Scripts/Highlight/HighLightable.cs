using UnityEngine;

public class HighLightable : MonoBehaviour
{
    public GameObject highlightObject;
    
    public bool isHighlighted;


    public void ToggleHighlight()
    {
        if (isHighlighted)
        {
            Unhighlight();
        }
        else
        {
            Highlight();
        }
    }
    public void Highlight()
    {
        if (highlightObject != null)
        {
            highlightObject.SetActive(true);
            isHighlighted = true;
        }
    }

    public void Unhighlight()
    {
        if (highlightObject != null)
        {
            highlightObject.SetActive(false);
            isHighlighted = false;
        }
    }

}