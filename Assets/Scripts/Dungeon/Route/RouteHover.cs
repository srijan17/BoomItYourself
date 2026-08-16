using UnityEngine;

//If mouse hovers over this gameObject then the route will be displayed
public class RouteHover : MonoBehaviour
{
    public bool isVisible = false; // Flag to track the visibility of the route
    public GameObject route;

    void OnMouseEnter()
    {
        // Show the route when the mouse hovers over the object
        isVisible = true;
        route.SetActive(true);
    }

    void OnMouseExit()
    {
        // Hide the route when the mouse stops hovering over the object
        isVisible = false;
        route.SetActive(false);
    }
}