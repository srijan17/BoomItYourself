using System.Collections;
using UnityEngine;

public class BombTarget : ITarget
{
    public override string targetName { get; set; }

    public GameObject bomb; 

    public GameObject explosionEffect; 

    public ExplosionCheck explosionRadiusCheck;

    public float explosionRadius; // The radius within which the explosion affects the hero
    private void Awake()
    {
        // Initialize the target name or any other necessary properties
        targetName = "BombTarget"; // Example target name
        SimulationManager.Instance.onNewEvent.AddListener(NewEvent);
    }


    public override void NewEvent(SimulationEvent simEvent)
    {
        if (simEvent.TargetName == targetName && simEvent.EventType == SimulationEventType.Explode)
        {
           Explode();
        }
    }

    public override void ResetTarget()
    {
        if(bomb!=null){
            bomb.SetActive(true); // Show the bomb object
        }
        if(explosionEffect!=null){
            explosionEffect.SetActive(false); // Hide the explosion effect
            explosionRadiusCheck.Deactivate();
            explosionRadiusCheck.gameObject.SetActive(false); // Hide the explosion radius check object
        }
        // Implement the behavior to reset the target
    }
    // private void checkHeroDied()
    // {
    //     // Check if the hero is within the explosion radius
    //     GameObject[] heroes = GameObject.FindGameObjectsWithTag("Hero");
    //     foreach (GameObject hero in heroes)
    //     {
    //         Debug.Log($"Checking hero: {hero.name}");
    //         if (hero != null)
    //         {
    //             float distance = Vector3.Distance(hero.transform.position, transform.position);
    //             Debug.Log($"Distance to hero: {distance}");
    //             if (distance <= explosionRadius)
    //             {
    //                //Call SimulationManager to notify that the hero has died
    //                SimulationManager.Instance.AddSimulationEvent(hero.name, SimulationEventType.HeroDied, SimulationManager.Instance.simulationElapsedTime);
    //             }
    //         }
    //     }
    // }
    private void Explode()
    {
        if(bomb!=null){
            bomb.SetActive(false); // Hide the bomb object
        }
        if(explosionEffect!=null){
            explosionEffect.SetActive(true); // Show the explosion effect
        }
        StartCoroutine(ExplosionRoutine());

    }

    

    private IEnumerator ExplosionRoutine()
    {
        Debug.Log("Explosion started");
        // particles / sound / animation

        explosionRadiusCheck.gameObject.SetActive(true);
        explosionRadiusCheck.Activate();

        yield return new WaitForFixedUpdate();

        explosionRadiusCheck.Deactivate();
    }
}