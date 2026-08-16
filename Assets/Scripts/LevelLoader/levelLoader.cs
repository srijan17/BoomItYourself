using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public struct LevelData
{
    public string levelName;
    public GameObject levelPrefab;
    public GameObject routePrefab;
    public GameObject heroPrefab;  
    public GameObject bombPrefab;
    }
public class LevelLoader : MonoBehaviour
{
    public List<LevelData> levels; // List of levels to load

    
    public GameObject levelParent; // Parent object to hold the loaded level
    public GameObject routeParent; // Parent object to hold the loaded route
    public GameObject heroParent; // Parent object to hold the loaded hero
    public void LoadLevel(LevelData levelData)
    {
        // Load the level prefab
        GameObject levelInstance = Instantiate(levelData.levelPrefab, levelParent.transform);
        levelInstance.name = levelData.levelName;

        // Load the route prefab
        GameObject routeInstance = Instantiate(levelData.routePrefab, routeParent.transform);
        routeInstance.name = levelData.levelName + "_Route";

        // Load the hero prefab
        GameObject heroInstance = Instantiate(levelData.heroPrefab, heroParent.transform);
        heroInstance.name = "Hero";
    }
}