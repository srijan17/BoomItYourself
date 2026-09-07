using UnityEngine;

public class EscapeCheck : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryHit(other);
    }

    private void TryHit(Collider other)
    {


        if (other.TryGetComponent<HeroTarget>(out var heroTarget))
        {
           Debug.Log("Hero hit by explosion");
           Debug.Log("Hero component: " + other.gameObject.name);
           heroTarget.Escape();
        }
    }
}