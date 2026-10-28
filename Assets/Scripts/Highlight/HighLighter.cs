using UnityEngine;

public class HighLighter : MonoBehaviour
{
    public HighLightable highLightable;

    public void ToggleHighlight()
    {
        if (highLightable != null)
        {
            highLightable.ToggleHighlight();
        }
    }

    public void Highlight()
    {
        if (highLightable != null)
        {
            highLightable.Highlight();
        }
    }

    public void Unhighlight()
    {
        if (highLightable != null)
        {
            highLightable.Unhighlight();
        }
    }
    
}