using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class NodeDefinition
{
    public ITimerNode timerNode;
    public string nodeName;
    public float nodeDuration;
}


[CreateAssetMenu]
public class LevelData : ScriptableObject
{
    public string[] startingNodes;
    public string bombNode;
    public string levelName;

    public GameObject dungeonPrefab;
    public GameObject[] heroPrefabs;
    public List<float> routeTimes; // List of times for each route in the level
    public float targetTime;

}
