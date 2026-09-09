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

[System.Serializable]
public class LevelOutputSignal
{
    public GameObject outputPrefab;
    public SimulationEventType outputType;
    public string targetName;
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

    //Add output signals for each level: prefab and type and target name
    //End timer,  gate1 Timer etc

    // Fix Initialize for timer nodes currently max Timer working incorrectly 

    // Plumb the workshop ouput trigger to dungeon gate etc. cause effect change routes etc 
    [SerializeField]
    public List<LevelOutputSignal> outputSignals;
}
