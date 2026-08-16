using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class JunctionConnection{
    public Junction nextJunction;
    public float time;
    public bool isActive;
}
[System.Serializable]
public class Junction : MonoBehaviour
{
    

    // [SerializeField] private bool isActive=true; // Indicates whether the junction is active or not
    [SerializeField] private List<JunctionConnection> nextJunctions; // List of next junctions connected to this junction
    
   

    public List<JunctionConnection> GetNextJunctions(){
        return nextJunctions;
    }
}