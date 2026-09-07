using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class LevelTimerNode
{
    public TimerNodeDefinition timer;
    public string timerName;
    public float maxDuration;
}

[CreateAssetMenu(menuName = "BIY/Timer Node Definition")]
public class TimerNodeDefinition : ScriptableObject
{
    public GameObject timerNode;
    public string timerID;
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
    public List<LevelTimerNode> timerNodes;
}
