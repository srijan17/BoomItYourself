using UnityEngine;
using System.Collections.Generic;
public class HeroRunner2 : MonoBehaviour
{

    public Junction startingJunction; // Reference to the starting junction in the route

    public JunctionConnection nextJunction; // Reference to the next junction in the route

    public Junction currentJunction; // Reference to the current junction in the route

    public RouteManager routeManager; // Reference to the RouteManager script

    public bool isRunning; // Flag to indicate if the hero is currently running

    // public float speed = 5f; // Speed at which the hero moves

    public float timeElapsedInSegment; // Time elapsed in the current segment of the route

    public void StartJourney()
    {
        Reset(); // Reset the hero's position and state
        nextJunction = routeManager.GetNextJunction(currentJunction); // Get the next junction in the route
        isRunning = true; // Set the running flag to true

    }

    public void Reset()
    {
        timeElapsedInSegment=0;
        currentJunction = startingJunction; // Reset the current junction to the starting junction
        nextJunction = null; // Reset the next junction to null
        isRunning = false; // Set the running flag to false
        transform.position = startingJunction.transform.position; // Reset the hero's position to the starting junction
    }

    public void Update(){

        if(isRunning){
            // Move the hero towards the next junction
            if(nextJunction != null){
                timeElapsedInSegment += Time.deltaTime; // Increment the time elapsed in the current segment
                float segmentDuration = nextJunction.time; // Duration of the current segment
                float t = timeElapsedInSegment / segmentDuration; // Calculate the interpolation factor based on
                transform.position = Vector3.Lerp(currentJunction.transform.position, nextJunction.nextJunction.transform.position, t); // Move the hero towards the next junction using linear interpolation
                // transform.position = Vector3.MoveTowards(transform.position, nextJunction.nextJunction.transform.position, speed * Time.deltaTime);
                //Turn the hero to face the next junction
                Vector3 direction = (nextJunction.nextJunction.transform.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(direction);
                }
        }
            hasReachedJunction(); // Check if the hero has reached the next junction
    }

    public void hasReachedJunction(){
        // Check if the hero has reached the next junction
       if(nextJunction != null && nextJunction.nextJunction != null){
        //Distance between hero and next junction world position
        float distance = Vector3.Distance(transform.position, nextJunction.nextJunction.transform.position);
        if(distance < 0.1f){ // Check if the hero is close enough to the next junction
            Debug.Log("Hero has reached the next junction: " + nextJunction.nextJunction.name);
            currentJunction = nextJunction.nextJunction; // Update the current junction to the next junction
            nextJunction = routeManager.GetNextJunction(currentJunction); // Get the next junction in the route
            timeElapsedInSegment=0; // Reset the time elapsed in the current segment
            if(nextJunction == null){
                isRunning = false; // Stop the hero from running if there are no more junctions
            }
        }
       }
    }
}