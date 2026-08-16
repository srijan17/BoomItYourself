using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class Route{
    public GameObject? lineObj;
    public Junction startingJunction;
    public Junction endingJunction;
    public float duration;
}
public class RouteManager : MonoBehaviour
{

    public GameObject HoverPrefab;
    public GameObject HoverPrefabParent;
    public Vector3 hoverPrefabOffset = new Vector3(0, -4, 0); // Offset for the hover prefab to position it above the junction
    public Vector3 lineOffset = new Vector3(0, 0.1f, 0); // Offset for the line renderer to avoid z-fighting with the ground
    public float lineWidth = 1f; // Width of the line renderer
    //LineRenderer Prefab to render the routes
    public GameObject lineRendererPrefab;
    public Junction startingJunction; // Reference to the starting junction in the route
    [SerializeField] public List<List<Junction>> allRoutes; // List to store all possible routes
    [SerializeField] public List<List<Route>> allJunctionConnections; // List to store all possible routes

    public List<Route> renderedRoutes; // List to store the rendered routes with their corresponding line objects and durations


    public TMPro.TextMeshProUGUI routeDurationsText; // Reference to the TextMeshPro text object to display route durations
    public GameObject MappingRouteRoot; // Root object to hold the mapped routes
    //random color list for routes 
    public List<Color> routeColors = new List<Color>(){
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        Color.white,
        Color.black
    };


    public void Awake(){
        // MapAllRoutes(); // Map all possible routes from the starting junction
        MapAllRoutesWithTimings(); // Map all possible routes with timings from the starting junction
        // RenderRoutes();
        Debug.Log("Total Routes Found: " + allJunctionConnections.Count);
        RenderRoutesWithRouteParents(); // Render the mapped routes with their corresponding line objects and durations
        DisplayRouteDurations(); // Display the durations of the rendered routes
    }

    public JunctionConnection GetNextJunction( Junction currentJunction){
        JunctionConnection nextConnection = null;
        if(currentJunction != null ){
            List<JunctionConnection> possibleNextJunctions = currentJunction.GetNextJunctions(); // Get the list of possible next junctions
            if(possibleNextJunctions.Count > 0){

              //Find first active junction in the list of possible next junctions
              nextConnection = possibleNextJunctions.Find(connection => connection.isActive);  
            }
        }

        return nextConnection; // Return the next junction connection
    }

    public void MapAllRoutes(){
        allRoutes = new List<List<Junction>>(); // List to store all possible routes
        //Start with start Junction and find all possible routes to the end junctions
        List<Junction> currentRoute = new List<Junction>(); // List to store the current route
        
        FindAllRoutes(startingJunction, currentRoute); // Start finding all routes from the starting junction
    
    }

    private void FindAllRoutes(Junction currentJunction, List<Junction> currentRoute)
    {
        currentRoute.Add(currentJunction); // Add the current junction to the current route

        List<JunctionConnection> nextJunctions = currentJunction.GetNextJunctions(); // Get the list of next junctions from the current junction

        if (nextJunctions.Count == 0)
        {

                allRoutes.Add(new List<Junction>(currentRoute)); // Add the current route to the list of all routes
        }
        else
        {
            // If there are next junctions, continue finding routes recursively
            foreach (JunctionConnection nextConnection in nextJunctions)
            {
                //If this junction is already part of current Route, then skip it to avoid infinite loop
                if (currentRoute.Contains(nextConnection.nextJunction))
                {
                    continue; // Skip this junction to avoid infinite loop
                }
                FindAllRoutes(nextConnection.nextJunction, new List<Junction>(currentRoute)); // Recursively find routes from the next junction
            }
        }
    }

    //Recursive function to find all routes with timings
    public void MapAllRoutesWithTimings(){
        allJunctionConnections = new List<List<Route>>(); // List to store all possible routes with timings
        //Start with start Junction and find all possible routes to the end junctions
        List<Route> currentRoute = new List<Route>(); // List to store the current route
        
        FindAllRoutesWithTimings(startingJunction, currentRoute); // Start finding all routes from the starting junction
    
    }
    private void FindAllRoutesWithTimings(Junction currentJunction, List<Route> currentRoute)
    {
        //currentRoute.Add(currentJunction); // Add the current junction to the current route
        List<JunctionConnection> nextJunctions = currentJunction.GetNextJunctions(); // Get the list of next junctions from the current junction

        if (nextJunctions.Count == 0)
        {

                allJunctionConnections.Add(new List<Route>(currentRoute)); // Add the current route to the list of all routes
        }
        else
        {
             // Create a new list to store the current route with timings
            // If there are next junctions, continue finding routes recursively
            foreach (JunctionConnection nextConnection in nextJunctions)
            {
                List<Route> newRoute = new List<Route>(currentRoute);
                //If this junction is already part of current Route, then skip it to avoid infinite loop
                Route route = new Route { startingJunction = currentJunction, 
                    endingJunction = nextConnection.nextJunction, 
                    duration = nextConnection.time 
                };
                if (currentRoute.Find(r => r.endingJunction == nextConnection.nextJunction && r.startingJunction == currentJunction) != null)
                {
                    continue; // Skip this junction to avoid infinite loop
                }
                newRoute.Add(route); // Add the current connection to the route
                FindAllRoutesWithTimings(nextConnection.nextJunction, new List<Route>(newRoute)); // Recursively find routes from the next junction
            }
        }
    }

    // public void RenderRoutes(){

    //     //Clean up any existing route lines
    //     foreach(Transform child in MappingRouteRoot.transform){
    //         Destroy(child.gameObject); // Destroy all child objects of the mapping route root
    //     }
    //     //Create LineRenderer between all junctions in all routes with Each route getting a new color
    //     int colorIndex = 0;
    //     foreach(List<Junction> route in allRoutes){
    //         Color routeColor = routeColors[colorIndex % routeColors.Count]; // Pick a color from the list of route colors
    //         colorIndex++;
    //         for(int i=0; i<route.Count-1; i++){
    //             Junction junctionA = route[i];
    //             Junction junctionB = route[i+1];
    //             GameObject lineObj = new GameObject("RouteLine"); // Create a new GameObject for the line
    //             lineObj.transform.parent = MappingRouteRoot.transform; // Set the parent of the line object to the mapping route root
    //             LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>(); // Add a LineRenderer component to the GameObject
    //             lineRenderer.positionCount = 2; // Set the number of positions in the LineRenderer
    //             lineRenderer.SetPosition(0, junctionA.transform.position); // Set the start position of the line
    //             lineRenderer.SetPosition(1, junctionB.transform.position); // Set the end position of the line
    //             lineRenderer.startWidth = 0.1f; // Set the start width of the line
    //             lineRenderer.endWidth = 0.1f; // Set the end width of the line
    //             lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Set the material for the LineRenderer
    //             lineRenderer.startColor = routeColor; // Set the start color of the line
    //             lineRenderer.endColor = routeColor; // Set the end color of the line
    //         }
    //     }
    // }

    
    public void RenderRoutesWithRouteParents(){

        //Clean up any existing route lines
        foreach(Transform child in MappingRouteRoot.transform){
            Destroy(child.gameObject); // Destroy all child objects of the mapping route root
        }
        //Create LineRenderer between all junctions in all routes with Each route getting a new color
        int colorIndex = 0;
        foreach(List<Route> routeConnections in allJunctionConnections){
            Color routeColor = routeColors[colorIndex % routeColors.Count]; // Pick a color from the list of route colors
            colorIndex++;   
            //Create a parent object for this route
            GameObject routeParent = new GameObject("RouteParent"); // Create a new GameObject
            routeParent.transform.parent = MappingRouteRoot.transform; // Set the parent of the route parent to the mapping route root
            GameObject lineObj = Instantiate(lineRendererPrefab, routeParent.transform); // Instantiate the line renderer prefab
            lineObj.name = "RouteLine"; // Set the name of the line object
            LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>(); // Get the LineRenderer component from the prefab
            lineRenderer.positionCount = routeConnections.Count+1; // Set the number of positions in the LineRenderer
            lineRenderer.endWidth = lineWidth; // Set the end width of the line
                // lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Set the material for the LineRenderer
                lineRenderer.startColor = routeColor; // Set the start color of the line
                lineRenderer.endColor = routeColor; // Set the end color of the line
                lineRenderer.startWidth = lineWidth; // Set the start width of the line
           
            for(int i=0; i<routeConnections.Count; i++){
                Junction junctionA = routeConnections[i].startingJunction;
                Junction junctionB = routeConnections[i].endingJunction;
                routeConnections[i].lineObj = lineObj; // Store the line object in the first route connection
                
                lineRenderer.SetPosition(i, junctionA.transform.position+lineOffset); // Set the start position of the line
                lineRenderer.SetPosition(i+1, junctionB.transform.position+lineOffset); // Set the end position of the line
              
            }
            //Instantiate hover prefab for this route
            if(HoverPrefab != null && HoverPrefabParent != null){
                GameObject hoverObj = Instantiate(HoverPrefab, HoverPrefabParent.transform); // Instantiate the hover prefab as a child of the hover prefab parent
                hoverObj.name = "RouteHover"; // Set the name of the hover object
                hoverObj.transform.localPosition = hoverPrefabOffset * (colorIndex - 1); // Position the first hover at the parent origin, then offset each subsequent hover
                RouteHover routeHover = hoverObj.GetComponent<RouteHover>(); // Get the RouteHover component from the prefab
                if(routeHover != null){
                    routeHover.route = routeParent; // Assign the route parent to the RouteHover component
                }
                routeParent.SetActive(false); // Disable the route parent initially, it will be enabled when hovered over
            }
        }
    }

    public void DisplayRouteDurations(){
        //Show Route name A , B C D . 
        //In format Route A = 10s => 5 + 3 +2
        if(routeDurationsText == null){
            Debug.LogError("Route Durations Text is not assigned in the RouteManager script.");
            return;
        }

        string routeDurations = "Route Durations:\n"; // Initialize the route durations string
        int routeIndex = 1; // Initialize the route index
        foreach(List<Route> routeConnections in allJunctionConnections){
            float totalDuration = 0f; // Initialize the total duration for the current route
            string routeName = "Route " + routeIndex + " = "; // Initialize the route name string
            string durationBreakdown = ""; // Initialize the duration breakdown string

            foreach(Route connection in routeConnections){
                totalDuration += connection.duration; // Add the duration of the current connection to the total duration
                durationBreakdown += connection.duration + " + "; // Append the duration of the current connection to the breakdown string
            }

            // if(durationBreakdown.Length > 3){
            //     durationBreakdown = durationBreakdown.Substring(0, durationBreakdown.Length - 3); // Remove the trailing " + " from the breakdown string
            // }

            routeDurations += routeName + totalDuration + "s => " + durationBreakdown + "\n"; // Append the current route's information to the overall route durations string
            routeIndex++; // Increment the route index for the next iteration
        }
        routeDurationsText.text = routeDurations; // Display the route durations in the TextMeshPro text object
    }
}