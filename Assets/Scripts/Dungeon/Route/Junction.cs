using UnityEngine;
using System.Collections.Generic;
using System.Linq;
[System.Serializable]
public enum RouteType{
    Normal,
    Door,
    Stairs
}
[System.Serializable]
public class JunctionConnection{
    public string routeId;
    public Junction nextJunction;
    public float time;
    public bool isActive;
    public RouteType routeType=RouteType.Normal;
}
[System.Serializable]
public class Junction : MonoBehaviour
{
    
    public string JunctionName;
    
    // [SerializeField] private bool isActive=true; // Indicates whether the junction is active or not
    [SerializeField] private List<JunctionConnection> nextJunctions; // List of next junctions connected to this junction
    
   

    public List<JunctionConnection> GetNextJunctions(){
        return nextJunctions;
    }

    public void ToggleRoute(string routeId, bool isActive)
    {
                // var connection = nextJunctions.FirstOrDefault(c => c.nextJunction.gameObject.name == targetJunction.gameObject.name);

        var connection = nextJunctions.FirstOrDefault(c => c.routeId == routeId);
        if (connection != null)
        {
            connection.isActive = isActive;
        }
    }
}