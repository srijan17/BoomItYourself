using UnityEngine;

public class ExplosionCheck : MonoBehaviour
{
    private bool active;
    public bool triggered=false;

    public void Activate()
    {
        active = true;
    }

    public void Deactivate()
    {
        active = false;
        gameObject.SetActive(false);
    }

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
        if (!active)
            return;

        if (other.TryGetComponent<HeroTarget>(out var heroTarget))
        {
           Debug.Log("Hero hit by explosion");
           Debug.Log("Hero component: " + other.gameObject.name);
           heroTarget.KillHero();
        }
    }
}