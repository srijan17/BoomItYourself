using UnityEngine;
using System.Collections.Generic;
public class DungeonRoot : MonoBehaviour
{
    private List<Junction> _Routes; // List of all junctions in the dungeon
    private List<ITarget> _Targets; // List of all targets in the dungeon

    public List<Junction> Routes => _Routes; // Public property to access the list of junctions
    public List<ITarget> Targets => _Targets; // Public property to access the list of targets
    private void Awake(){
        _Routes = new List<Junction>(GetComponentsInChildren<Junction>());
        _Targets = new List<ITarget>(GetComponentsInChildren<ITarget>());
    }


    public Junction GetJunctionByName(string junctionName){
        return _Routes.Find(j => j.name == junctionName);
    }
    public void AllocateTimesToRoute(List<float> times){
        Debug.Log($"Allocating times to routes: {string.Join(", ", times)} for {_Routes.Count} routes.");
        if(times.Count != _Routes.Count){
            Debug.LogError("Number of times provided does not match the number of routes.");
            return;
        }
        int timerIndex = 0;
        for(int i=0; i<_Routes.Count; i++){
            Junction junction = _Routes[i];
            // Assuming each junction has a method to set its time
            foreach(var connection in junction.GetNextJunctions()){
                connection.time = times[timerIndex];
                timerIndex++;
            }
        }
    }

}