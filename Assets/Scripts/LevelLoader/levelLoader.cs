using UnityEngine;
using System.Collections.Generic;



public class LevelLoader : MonoBehaviour
{   public int currentLevelIndex=-1;
    public List<LevelData> levels; // List of levels to load

    public SimulationManager simulationManager; // Reference to the SimulationManager
    public RouteManager routeManager; // Reference to the RouteManager
    public GameObject levelParent; // Parent object to hold the loaded level
    public GameObject routeParent; // Parent object to hold the loaded route
    public GameObject heroParent; // Parent object to hold the loaded hero
    public LevelData currLevelData;
    public GameObject bombPrefab; // Reference to the bomb prefab

    public GameObject timerRoot; // Parent object to hold the loaded timer nodes
    public GameObject endTimerRoot; // Parent object to hold the loaded end timer nodes

    [SerializeField] public List<Transform> TimerNodeSpawnPoints;
    [SerializeField] public List<Transform> EndTimerNodeSpawnPoints;

    //parent for timer loading
    // level data needs all timer nodes
    // generate timer nodes based on the level data
    // reset handles that 
    

    //TEMPORARY  WILL ADD END NODE INSTANTIATE AND GAME NODE ISNTANTE
    public EndTimer endTimer;
    public void ResetAndLoadHero()
    {
        DungeonRoot dungeonRoot = levelParent.GetComponentInChildren<DungeonRoot>();
        if(currLevelData != null && dungeonRoot != null)
        {
            ResetHero(currLevelData, dungeonRoot);
        }
        InstantiateTimerNodes();
        InstantiateBomb(currLevelData.bombNode, dungeonRoot);
        InstantiateOutput();
    }

    public void InstantiateOutput(){
         DungeonRoot dungeonRoot = levelParent.GetComponentInChildren<DungeonRoot>();
        //Clear existing end nodes
        EndTimer[] existingendTimerNodes = endTimerRoot.GetComponentsInChildren<EndTimer>();

        List<ITarget> targets = new List<ITarget>(levelParent.gameObject.GetComponentsInChildren<ITarget>());

        if (existingendTimerNodes != null)
        {
            foreach (EndTimer timerNode in existingendTimerNodes)
            {
                Destroy(timerNode.gameObject);
            }
        }
        if(currLevelData != null && dungeonRoot != null)
        {
            for(int i = 0; i < currLevelData.outputSignals.Count; i++)
            {
                LevelOutputSignal endTimerNodeData = currLevelData.outputSignals[i];
              
                    Transform spawnPoint = EndTimerNodeSpawnPoints[i];
                    GameObject endTimerInstance = Instantiate(endTimerNodeData.outputPrefab,  spawnPoint.position, Quaternion.identity);
                    endTimerInstance.transform.SetParent(endTimerRoot.transform);

                    EndTimer endTimerComponent = endTimerInstance.GetComponent<EndTimer>();

                    //Find relevant target for the end timer node
                    ITarget target = targets.Find(t => t.gameObject.name == endTimerNodeData.targetName);
                    if(target != null)
                    {
                        endTimerComponent.Target=target;
                    endTimerComponent.eventType = endTimerNodeData.outputType;
                    }
            }
        }
    }

    public void InstantiateTimerNodes(){
                DungeonRoot dungeonRoot = levelParent.GetComponentInChildren<DungeonRoot>();
                //Clear existing timer nodes
        ITimerNode[] existingTimerNodes = timerRoot.GetComponentsInChildren<ITimerNode>();
        if (existingTimerNodes != null)
        {
            foreach (ITimerNode timerNode in existingTimerNodes)
            {
                Destroy(timerNode.gameObject);
            }
        }
        if(currLevelData != null && dungeonRoot != null)
        {
            for(int i = 0; i < currLevelData.timerNodes.Count; i++)
            {
                LevelTimerNode timerNodeData = currLevelData.timerNodes[i];
                if(timerNodeData.timer != null && TimerNodeSpawnPoints.Count > i)
                {
                    Transform spawnPoint = TimerNodeSpawnPoints[i];
                    GameObject timerNodeInstance = Instantiate(timerNodeData.timer.timerNode, spawnPoint.position, Quaternion.identity);
                    timerNodeInstance.name = timerNodeData.timerName;
                    ITimerNode timerNodeComponent = timerNodeInstance.GetComponent<ITimerNode>();
                    timerNodeComponent.maxDuration=timerNodeData.maxDuration;
                    timerNodeInstance.transform.SetParent(timerRoot.transform);
                }
            }
        }

    }
    private void ResetHero(LevelData levelData, DungeonRoot dungeonRoot)
    {
        
        // Destroy any existing hero instances
        HeroRunner2[] existingHeroes = heroParent.GetComponentsInChildren<HeroRunner2>();
        if (existingHeroes != null)
        {
            foreach (HeroRunner2 hero in existingHeroes)
            {
                Destroy(hero.gameObject);
            }
        }
        for (int i = 0; i < levelData.heroPrefabs.Length; i++)
        {
            GameObject heroInstance = Instantiate(levelData.heroPrefabs[i], heroParent.transform);
            heroInstance.name = "Hero";
            Junction startingJunction = dungeonRoot.GetJunctionByName(levelData.startingNodes[i]);
            HeroRunner2 heroRunner = heroInstance.GetComponent<HeroRunner2>();
            if (heroRunner != null){
                heroRunner.startingJunction = startingJunction; // Set the starting junction for the hero
                heroRunner.routeManager = routeManager; // Set the reference to the RouteManager
            }
            else{
                Debug.LogError("HeroRunner2 component not found on the hero prefab.");
            }
        }

        routeManager.startingJunction = dungeonRoot.Routes[0]; // Set the starting junction for the route manager
    }

    public void InstantiateBomb(string JunctionName,DungeonRoot dungeonRoot){
        //Delete ExistingBombs
         BombTarget[] existingBombs = dungeonRoot.gameObject.GetComponentsInChildren<BombTarget>();
        if (existingBombs != null)
        {
            foreach (BombTarget bomb in existingBombs)
            {
                Destroy(bomb.gameObject);
            }
        }
        Junction targetJunction = dungeonRoot.GetJunctionByName(JunctionName);
        if(targetJunction != null){
            // Instantiate the bomb at the target junction
            // Assuming you have a bomb prefab and a method to place it at the junction
            GameObject bombInstance = Instantiate(bombPrefab, targetJunction.transform.position, Quaternion.identity);
            bombInstance.name = "Bomb";
            bombInstance.transform.SetParent(dungeonRoot.transform); // Set the bomb as a child of the dungeon root
            endTimer.Target = bombInstance.GetComponent<BombTarget>();
        }
        else{
            Debug.LogError($"Junction with name {JunctionName} not found in the dungeon.");
        }
    }
    public void LoadLevel(LevelData levelData)
    {
        if(currentLevelIndex+1 < levels.Count){
            currentLevelIndex++;
        }

        levelData = levels[currentLevelIndex];
        // Destroy any existing level instances
        foreach (Transform child in levelParent.transform)
        {
            Destroy(child.gameObject);
        }
        currLevelData = levelData;
        // Load the level prefab
        GameObject levelInstance = Instantiate(levelData.dungeonPrefab, levelParent.transform);
        levelInstance.name = levelData.levelName;

        DungeonRoot dungeonRoot = levelInstance.GetComponent<DungeonRoot>();
        if (dungeonRoot != null && levelData.routeTimes.Count > 0)
        {
            // Allocate times to the routes in the dungeon
            dungeonRoot.AllocateTimesToRoute(levelData.routeTimes);
        }
        else
        {
            Debug.LogError("DungeonRoot component not found on the level prefab.");
        }
       ResetHero(levelData, dungeonRoot);
        InstantiateBomb(levelData.bombNode, dungeonRoot);
        InstantiateTimerNodes();
        InstantiateOutput();
        // dungeonRoot.Routes[0];
            
        // Load the route prefab
        // GameObject routeInstance = Instantiate(levelData.routePrefab, routeParent.transform);
        // routeInstance.name = levelData.levelName + "_Route";

        // Load the hero prefab

    }
}